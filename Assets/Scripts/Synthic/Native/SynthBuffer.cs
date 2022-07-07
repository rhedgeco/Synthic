using System;
using System.Runtime.InteropServices;

namespace Synthic.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SynthBuffer : IDisposable
    {
        private BufferHandler<float> _buffer;

        public int Channels { get; }
        public int ChannelLength { get; }
        public int Length => _buffer.Length;
        public bool Allocated => _buffer.Allocated;

        public SynthBuffer(int bufferLength, int channels)
        {
            Channels = channels;
            ChannelLength = bufferLength / channels;
            _buffer = new BufferHandler<float>(bufferLength);
        }

        public float this[int index]
        {
            get => _buffer[index];
            set => _buffer[index] = value;
        }

        public void CopyTo(float[] managedArray) => _buffer.CopyTo(managedArray);
        public void CopyTo(ref SynthBuffer buffer) => _buffer.CopyTo(buffer._buffer);

        public void Dispose()
        {
            _buffer.Dispose();
        }
    }
}