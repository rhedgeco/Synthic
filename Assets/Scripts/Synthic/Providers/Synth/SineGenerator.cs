using Synthic.Data;
using Synthic.Native;
using Synthic.Native.Core;
using Synthic.Native.Midi;
using Synthic.Providers.Midi;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace Synthic.Providers.Synth
{
    [BurstCompile]
    public class SineGenerator : SynthProvider
    {
        [SerializeField] private MidiProvider midiProvider;
        [SerializeField, Range(0, 1)] private float amplitude;
        [SerializeField] private Adsr adsr;

        private static BurstSineDelegate _burstSine;

        private int _sampleRate;
        private NativeBox<DataBuffer<NoteState>> _noteStates;

        private void Awake()
        {
            _sampleRate = AudioSettings.GetConfiguration().sampleRate;
            _noteStates = DataBuffer<NoteState>.Construct(128);

            if (_burstSine != null) return;
            _burstSine = BurstCompiler.CompileFunctionPointer<BurstSineDelegate>(BurstSine).Invoke;
        }

        protected override void ProcessBuffer(ref SynthBuffer buffer)
        {
            ref MidiBuffer midiBuffer = ref midiProvider.ReadBuffer(buffer.ChannelLength);
            _burstSine(ref buffer, ref midiBuffer, ref _noteStates.Data, ref adsr, amplitude, _sampleRate);
        }

        private delegate void BurstSineDelegate(ref SynthBuffer synthBuffer, ref MidiBuffer midiBuffer,
            ref DataBuffer<NoteState> noteStates, ref Adsr adsr, float amplitude, int sampleRate);

        [BurstCompile]
        private static void BurstSine(ref SynthBuffer synthBuffer, ref MidiBuffer midiBuffer,
            ref DataBuffer<NoteState> noteStates, ref Adsr adsr, float amplitude, int sampleRate)
        {
            float sampleTime = 1f / sampleRate;

            // loop over every sample
            int channelLength = synthBuffer.ChannelLength;
            for (int sample = 0; sample < channelLength; sample++)
            {
                float sampleValue = 0;

                // turn on and off necessary notes
                MidiPacket packet = midiBuffer.GetPacket(sample);
                for (int nodeIndex = 0; packet.Allocated & nodeIndex < packet.Length; nodeIndex++)
                {
                    ref MidiNote note = ref packet[nodeIndex];
                    ref NoteState state = ref noteStates[note.NoteIndex];
                    if (note.MidiSignal == MidiNote.Signal.On) state.TurnOn(0, note.Velocity / 255f, in adsr);
                    else state.TurnOff();
                }

                // calculate waveform as combination of all waves
                for (int noteIndex = 0; noteIndex < noteStates.Length; noteIndex++)
                {
                    ref NoteState state = ref noteStates[noteIndex];
                    if (state.NetVelocity == 0 && state.Signal == MidiNote.Signal.Off) continue;

                    sampleValue += (float) math.sin(state.Phase * 2 * math.PI) * state.NetVelocity * amplitude;

                    state.Advance(sampleTime, Frequency.ConvertFromMidi((byte) noteIndex) / sampleRate);
                }

                for (int channel = 0; channel < synthBuffer.Channels; channel++)
                {
                    synthBuffer[sample * synthBuffer.Channels + channel] = sampleValue;
                }
            }
        }
    }
}