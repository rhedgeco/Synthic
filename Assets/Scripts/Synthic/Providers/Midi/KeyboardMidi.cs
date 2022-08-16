using System.Collections.Generic;
using Synthic.Native.Midi;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Synthic.Providers.Midi
{
    public class KeyboardMidi : MidiProvider
    {
        [SerializeField] private InputAction noteAction;
        
        private readonly object _notesLock = new object();
        private readonly List<MidiNote> _notes = new List<MidiNote>();

        private void Awake()
        {
            noteAction.Enable();
            noteAction.started += context => { lock (_notesLock) { _notes.Add(MidiNote.On(64, 255)); } };
            noteAction.canceled += context => { lock (_notesLock) { _notes.Add(MidiNote.Off(64)); } };
        }

        protected override void ProcessBuffer(ref MidiBuffer buffer)
        {
            lock (_notesLock)
            {
                buffer.Clear();
                for (int sample = 0; sample < buffer.Length; sample++)
                {
                    if (_notes.Count == 0) continue;
                    buffer.SetPacket(sample, _notes.ToArray());
                    _notes.Clear();
                }
            }
        }
    }
}