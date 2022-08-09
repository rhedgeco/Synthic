using Synthic.Native;
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
            _burstSine(ref buffer, ref midiBuffer, ref _noteStates.Data, amplitude, _sampleRate);
        }

        private delegate void BurstSineDelegate(ref SynthBuffer synthBuffer, ref MidiBuffer midiBuffer,
            ref DataBuffer<NoteState> noteStates, float amplitude, int sampleRate);

        [BurstCompile]
        private static void BurstSine(ref SynthBuffer synthBuffer, ref MidiBuffer midiBuffer,
            ref DataBuffer<NoteState> noteStates, float amplitude, int sampleRate)
        {
            // loop over each sample
            for (int sample = 0; sample < synthBuffer.ChannelLength; sample++)
            {
                float sampleValue = 0;
                
                // update note states for this iteration
                MidiPacket packet = midiBuffer.GetPacket(sample);
                for (int noteIndex = 0; packet.Allocated & noteIndex < packet.Length; noteIndex++)
                {
                    ref MidiNote note = ref packet[noteIndex];
                    ref NoteState state = ref noteStates[note.NoteIndex];
                    state.Signal = note.MidiSignal;
                    if (note.MidiSignal == MidiNote.Signal.Off) continue;

                    state.Phase = 0;
                    state.Velocity = note.Velocity;
                }

                // iterate over all notes to check signal
                for (int noteIndex = 0; noteIndex < noteStates.Length; noteIndex++)
                {
                    // check if state is on or off
                    ref NoteState state = ref noteStates[noteIndex];
                    if (state.Signal == MidiNote.Signal.Off) continue;
                    
                    // process sample value for note
                    float value = (float) math.sin(state.Phase * 2 * math.PI);
                    
                    // advance note phase
                    state.Phase += Frequency.ConvertFromMidi((byte) noteIndex) / sampleRate;
                    
                    // apply note value to total sample calculation
                    sampleValue += value * state.VelocityFloat * amplitude;
                }

                for (int channel = 0; channel < synthBuffer.Channels; channel++)
                {
                    synthBuffer[sample * synthBuffer.Channels + channel] = sampleValue;
                }
            }
        }
    }
}