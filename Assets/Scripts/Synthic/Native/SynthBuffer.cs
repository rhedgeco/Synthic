using System;
using System.Runtime.InteropServices;

namespace Synthic.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SynthBuffer : IDisposable
    {
        public int Channels { get; }
        public int ChannelLength { get; }
        public BufferHandler<float> Handler { get; }

        public SynthBuffer(int bufferLength, int channels)
        {
            Channels = channels;
            ChannelLength = bufferLength / channels;
            Handler = new BufferHandler<float>(bufferLength);
        }

        public void Dispose()
        {
            Handler.Dispose();
        }
    }
}