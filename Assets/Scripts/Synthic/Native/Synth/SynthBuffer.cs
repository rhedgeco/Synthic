using System;
using System.Runtime.InteropServices;
using Synthic.Native.Core;
using Unity.Collections.LowLevel.Unsafe;

namespace Synthic.Native.Synth
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SynthBuffer : INativeObject
    {
        private BufferHandler<StereoData> _buffer;

        public const int Channels = 2;
        public int ChannelLength { get; private set; }
        public int Length => _buffer.Length;
        public bool Allocated => _buffer.Allocated;

        public static NativeBox<SynthBuffer> Construct(int bufferLength)
        {
            SynthBuffer buffer = new SynthBuffer
            {
                ChannelLength = bufferLength / Channels,
                _buffer = new BufferHandler<StereoData>(bufferLength)
            };
            return new NativeBox<SynthBuffer>(buffer);
        }

        public ref StereoData this[int index] => ref _buffer[index];


        public unsafe void CopyTo(float[] managedArray)
        {
            if (managedArray == null) throw new NullReferenceException("Cannot copy. Managed array is null");
            if (!Allocated) throw new ObjectDisposedException("Cannot copy. Buffer has been disposed");
            int length = Math.Min(managedArray.Length, Length * 2);
            GCHandle gcHandle = GCHandle.Alloc(managedArray, GCHandleType.Pinned);
            UnsafeUtility.MemCpy((float*) gcHandle.AddrOfPinnedObject(), _buffer.Pointer,
                length * sizeof(float));
            gcHandle.Free();
        }

        public void CopyTo(ref SynthBuffer buffer) => _buffer.CopyTo(buffer._buffer);
        public BufferRefIterator<StereoData> GetIterator() => new BufferRefIterator<StereoData>(ref _buffer);

        void INativeObject.ReleaseResources() => _buffer.Dispose();
    }
}