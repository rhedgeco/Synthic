using System;
using Buffer = Synthic.Native.Buffer;

namespace Synthic.Handlers
{
    public struct BufferView<T> where T : unmanaged
    {
        internal unsafe Buffer* NativePtr;
        private readonly long _viewId;
        private readonly unsafe T* _ptr;

        public int Length { get; }

        public ref T this[int index]
        {
            get
            {
                if (_viewId != SynthicEngine.CurrentViewId)
                    throw new InvalidBufferView("Buffer views are invalidated when the global buffer size changes");
                if (index >= Length || index < 0)
                    throw new IndexOutOfRangeException();
                unsafe
                {
                    return ref _ptr[index];
                }
            }
        }

        public BufferView(Buffer<T> buffer)
        {
            unsafe
            {
                NativePtr = buffer.NativePtr;
                _viewId = SynthicEngine.CurrentViewId;
                Length = buffer.Length;
                _ptr = buffer.GetBufferPtr();
            }
        }
    }
}
