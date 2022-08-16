using System.Runtime.InteropServices;

namespace Synthic.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SynthBuffer : INativeObject
    { 
        private BufferHandler<float> _buffer;
        
        public int Channels { get; private set; }
        public int ChannelLength { get; private set; }
        public int Length => _buffer.Length;
        public bool Allocated => _buffer.Allocated;

        public static NativeBox<SynthBuffer> Construct(int bufferLength, int channels)
        {
            SynthBuffer buffer = new SynthBuffer
            {
                Channels = channels,
                ChannelLength = bufferLength / channels,
                _buffer = new BufferHandler<float>(bufferLength)
            };
            return new NativeBox<SynthBuffer>(buffer);
        }
        
        public float this[int index]
        {
            get => _buffer[index];
            set => _buffer[index] = value;
        }

        public void CopyTo(float[] managedArray) => _buffer.CopyTo(managedArray);
        public void CopyTo(ref SynthBuffer buffer) => _buffer.CopyTo(buffer._buffer);

        void INativeObject.ReleaseResources() => _buffer.Dispose();
    }
}