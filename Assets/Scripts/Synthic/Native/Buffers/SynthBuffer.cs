using System;
using System.Runtime.InteropServices;
using Synthic.Native.Data;
using Unity.Collections.LowLevel.Unsafe;

namespace Synthic.Native.Buffers
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SynthBuffer : INativeObject
    { 
        private BufferHandler<StereoData> _buffer;
        
        public int Length => _buffer.Length;
        public bool Allocated => _buffer.Allocated;

        public static NativeBox<SynthBuffer> Construct(int bufferLength)
        {
            return new NativeBox<SynthBuffer>(new SynthBuffer {_buffer = new BufferHandler<StereoData>(bufferLength)});
        }
        
        public StereoData this[int index]
        {
            get => _buffer[index];
            set => _buffer[index] = value;
        }

        public unsafe void CopyToManaged(float[] managedArray)
        {
            if (!Allocated) throw new ObjectDisposedException("Cannot copy. Buffer has been disposed");
            int length = Math.Min(managedArray.Length, Length * 2);
            GCHandle gcHandle = GCHandle.Alloc(managedArray, GCHandleType.Pinned);
            UnsafeUtility.MemCpy((void*) gcHandle.AddrOfPinnedObject(), _buffer.Pointer, length * sizeof(StereoData));
            gcHandle.Free();
        }
        
        public void CopyTo(ref SynthBuffer buffer) => _buffer.CopyTo(buffer._buffer);

        void INativeObject.ReleaseResources() => _buffer.Dispose();
    }
}