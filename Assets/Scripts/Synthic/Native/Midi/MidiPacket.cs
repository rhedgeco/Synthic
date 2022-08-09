using System.Runtime.InteropServices;

namespace Synthic.Native.Midi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MidiPacket : INativeObject
    {
        private BufferHandler<MidiNote> _buffer;

        public int Length => _buffer.Length;
        public bool Allocated => _buffer.Allocated;

        internal void Allocate(MidiNote[] packet) => _buffer = new BufferHandler<MidiNote>(packet);

        public ref MidiNote this[int index] => ref _buffer[index];

        public void Clear() => _buffer.Clear();

        void INativeObject.ReleaseResources() => _buffer.Dispose();
    }
}