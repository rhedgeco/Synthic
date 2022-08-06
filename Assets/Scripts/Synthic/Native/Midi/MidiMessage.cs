using System.Collections.Generic;

namespace Synthic.Native.Midi
{
    public struct MidiMessage : INativeObject
    {
        private BufferHandler<MidiNote> _notes;

        public bool Allocated => _notes.Allocated;

        public static NativeBox<MidiMessage> Construct(IReadOnlyCollection<MidiNote> notes)
        {
            MidiMessage message = new MidiMessage {_notes = new BufferHandler<MidiNote>(notes)};
            return new NativeBox<MidiMessage>(message);
        }

        void INativeObject.ReleaseResources() => _notes.Dispose();
    }
}