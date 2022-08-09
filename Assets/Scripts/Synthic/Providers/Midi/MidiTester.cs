using System.Collections.Generic;
using Minis;
using Synthic.Native.Midi;
using UnityEngine.InputSystem;

namespace Synthic.Providers.Midi
{
    public class MidiTester : MidiProvider
    {
        private List<MidiNote> _notes = new List<MidiNote>();

        private void Awake()
        {
            InputSystem.onDeviceChange += (device, change) =>
            {
                if (change != InputDeviceChange.Added) return;

                MidiDevice midiDevice = device as MidiDevice;
                if (midiDevice == null) return;

                midiDevice.onWillNoteOn += (note, velocity) =>
                    _notes.Add(MidiNote.On((byte) note.noteNumber, (byte) (127 * velocity)));

                midiDevice.onWillNoteOff += note => _notes.Add(MidiNote.Off((byte) note.noteNumber));
            };
        }

        protected override void ProcessBuffer(ref MidiBuffer buffer)
        {
            buffer.Clear();
            for (int sample = 0; sample < buffer.Length; sample++)
            {
                if (_notes.Count == 0) continue;
                buffer.ApplyPacket(sample, _notes.ToArray());
                _notes.Clear();
            }
        }
    }
}