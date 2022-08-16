using System.Collections.Generic;
using Minis;
using Synthic.Native.Midi;
using UnityEngine.InputSystem;

namespace Synthic.Providers.Midi
{
    public class MidiTester : MidiProvider
    {
        private readonly object _notesLock = new object();
        private readonly List<MidiNote> _notes = new List<MidiNote>();
        private bool _clearBuffer = false;
        private int _localCount = 0;

        private void Awake()
        {
            InputSystem.onDeviceChange += (device, change) =>
            {
                if (change != InputDeviceChange.Added) return;

                MidiDevice midiDevice = device as MidiDevice;
                if (midiDevice == null) return;

                midiDevice.onWillNoteOn += (note, velocity) =>
                {
                    lock (_notesLock)
                    {
                        _notes.Add(MidiNote.On((byte) note.noteNumber, (byte) (127 * velocity)));
                        _localCount++;
                    }
                };

                midiDevice.onWillNoteOff += note =>
                {
                    lock (_notesLock)
                    {
                        _notes.Add(MidiNote.Off((byte) note.noteNumber));
                        _localCount++;
                    }
                };
            };
        }

        protected override void ProcessBuffer(ref MidiBuffer buffer)
        {
            lock (_notesLock)
            {
                if (_clearBuffer)
                {
                    buffer.Clear();
                    _clearBuffer = false;
                }

                int bufferLength = buffer.Length;
                for (int sample = 0; sample < bufferLength; sample++)
                {
                    if (_localCount == 0) continue;
                    buffer.SetPacket(sample, _notes.ToArray());
                    _notes.Clear();
                    _localCount = 0;
                    _clearBuffer = true;
                }
            }
        }
    }
}