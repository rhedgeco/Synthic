using System.Runtime.InteropServices;
using Synthic.Native.Core;

namespace Synthic.Native.Midi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MidiBuffer : INativeObject
    {
        private BufferHandler<MidiPacket> _packets;

        public int Length => _packets.Length;
        public bool Allocated => _packets.Allocated;

        public static NativeBox<MidiBuffer> Construct(int bufferLength)
        {
            MidiBuffer buffer = new MidiBuffer {_packets = new BufferHandler<MidiPacket>(bufferLength)};
            return new NativeBox<MidiBuffer>(buffer);
        }

        public void SetPacket(int index, MidiNote[] notes)
        {
            ref MidiPacket packet = ref _packets[index];
            if (packet.Allocated) packet.Dispose();
            _packets[index] = new MidiPacket(notes);
        }

        public ref MidiPacket GetPacket(int index)
        {
            return ref _packets[index];
        }

        public void Clear()
        {
            for (int i = 0; i < _packets.Length; i++)
            {
                ref MidiPacket packet = ref _packets[i];
                if (!packet.Allocated) continue;
                packet.Dispose();
            }
        }

        void INativeObject.ReleaseResources()
        {
            for (int i = 0; i < _packets.Length; i++)
            {
                ref MidiPacket packet = ref _packets[i];
                if (!packet.Allocated) continue;
                packet.Dispose();
            }

            _packets.Dispose();
        }
    }
}