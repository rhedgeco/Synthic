using System.Runtime.InteropServices;
using Synthic.Native.Core;

namespace Synthic.Native.Midi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MidiBuffer : INativeObject
    {
        private BufferHandler<MidiPacket> _buffer;

        public int Length => _buffer.Length;
        public bool Allocated => _buffer.Allocated;

        public static NativeBox<MidiBuffer> Construct(int bufferLength)
        {
            MidiBuffer buffer = new MidiBuffer {_buffer = new BufferHandler<MidiPacket>(bufferLength)};
            return new NativeBox<MidiBuffer>(buffer);
        }

        public void SetPacket(int index, MidiNote[] notes)
        {
            ref MidiPacket packet = ref _buffer[index];
            if (packet.Allocated) packet.Dispose();
            _buffer[index] = new MidiPacket(notes);
        }

        public ref MidiPacket GetPacket(int index)
        {
            return ref _buffer[index];
        }

        public BufferRefIterator<MidiPacket> GetIterator() => _buffer.GetIterator();

        public void Clear()
        {
            for (int i = 0; i < _buffer.Length; i++)
            {
                ref MidiPacket packet = ref _buffer[i];
                if (!packet.Allocated) continue;
                packet.Dispose();
            }
        }

        void INativeObject.ReleaseResources()
        {
            for (int i = 0; i < _buffer.Length; i++)
            {
                ref MidiPacket packet = ref _buffer[i];
                if (!packet.Allocated) continue;
                packet.Dispose();
            }

            _buffer.Dispose();
        }
    }
}