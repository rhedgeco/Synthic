using System.Runtime.InteropServices;
using Synthic.Native.Core;

namespace Synthic.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DataBuffer<T> : INativeObject where T : unmanaged
    {
        private BufferHandler<T> _buffer;

        public int Length => _buffer.Length;
        public bool Allocated => _buffer.Allocated;

        public static NativeBox<DataBuffer<T>> Construct(int bufferLength)
        {
            DataBuffer<T> buffer = new DataBuffer<T> {_buffer = new BufferHandler<T>(bufferLength)};
            return new NativeBox<DataBuffer<T>>(buffer);
        }

        public ref T this[int index] => ref _buffer[index];

        public void CopyTo(T[] managedArray) => _buffer.CopyTo(managedArray);
        public void CopyTo(ref DataBuffer<T> buffer) => _buffer.CopyTo(buffer._buffer);

        void INativeObject.ReleaseResources() => _buffer.Dispose();
    }
}