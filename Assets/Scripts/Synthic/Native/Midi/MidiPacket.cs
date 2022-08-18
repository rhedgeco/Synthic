using System.Runtime.InteropServices;
using Synthic.Native.Core;

namespace Synthic.Native.Midi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MidiPacket
    {
        private BufferHandler<MidiNote> _buffer;

        public int Length => _buffer.Length;
        public bool Allocated => _buffer.Allocated;

        internal MidiPacket(MidiNote[] notes)
        {
            _buffer = new BufferHandler<MidiNote>(notes);
        }

        public ref MidiNote this[int index] => ref _buffer[index];
        public BufferRefIterator<MidiNote> GetIterator() => _buffer.GetIterator();

        internal void Dispose() => _buffer.Dispose();
    }
}