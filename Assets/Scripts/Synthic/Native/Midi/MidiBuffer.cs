using System.Runtime.InteropServices;
using Unity.Burst;

namespace Synthic.Native.Midi
{
    [BurstCompile]
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

        public void ApplyPacket(int index, MidiNote[] notes)
        {
            _buffer[index].Allocate(notes);
        }

        public MidiPacket GetPacket(int index)
        {
            return _buffer[index];
        }

        public void Clear()
        {
            for (int packetIndex = 0; packetIndex < _buffer.Length; packetIndex++)
            {
                ref MidiPacket packet = ref _buffer[packetIndex];
                if (!packet.Allocated) continue;
                packet.Clear();
            }
        }

        void INativeObject.ReleaseResources()
        {
            for (int packetIndex = 0; packetIndex < _buffer.Length; packetIndex++)
            {
                ref MidiPacket packet = ref _buffer[packetIndex];
                if (!packet.Allocated) continue;
                ((INativeObject) packet).ReleaseResources();
            }
            _buffer.Dispose();
        }
    }
}