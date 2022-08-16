using System.Runtime.InteropServices;

namespace Synthic.Native.Midi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MidiPacket
    {
        private BufferHandler<MidiNote> _notes;

        public int Length => _notes.Length;
        public bool Allocated => _notes.Allocated;

        internal MidiPacket(MidiNote[] notes)
        {
            _notes = new BufferHandler<MidiNote>(notes);
        }

        public ref MidiNote this[int index] => ref _notes[index];

        internal void Dispose() => _notes.Dispose();
    }
}