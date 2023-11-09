using System.Collections.Generic;
using Minis;
using Synthic.Handlers.Data;
using Unity.Mathematics;
using UnityEngine.InputSystem;

namespace Synthic.Generators
{
    public class MidiNotesGenerator : BufferNode
    {
        public const float A = 440;
        private const float ADiv32 = A / 32f;

        private Notes _currentState;

        private void Awake()
        {
            InputSystem.onDeviceChange += (device, change) =>
            {
                if (change != InputDeviceChange.Added) return;
                var midiDevice = device as MidiDevice;
                if (midiDevice == null) return;

                midiDevice.onWillNoteOn += (note, velocity) =>
                {
                    _currentState.Set(note.noteNumber, velocity, FrequencyFromMidi(note.noteNumber));
                };

                midiDevice.onWillNoteOff += note => { _currentState.GetAmplitude(note.noteNumber) = 0; };
            };
        }

        protected override IEnumerable<OutputPort> RegisterOutputs()
        {
            yield return OutputPort.Construct<Notes>("notes");
        }

        protected override void ProcessCache(long sampleTime, BufferCache bufferCache)
        {
            var notesCache = bufferCache.GetChannelBuffer<Notes>(0);
            notesCache.SetAllValues(_currentState);
        }

        public static float FrequencyFromMidi(int midiNote)
        {
            return ADiv32 * math.pow(2, (midiNote - 9) / 12f);
        }
    }
}
