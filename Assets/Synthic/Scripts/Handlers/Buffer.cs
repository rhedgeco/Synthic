using System;
using Synthic.Native;
using Unity.Collections.LowLevel.Unsafe;
using Buffer = Synthic.Native.Buffer;

namespace Synthic.Handlers
{
    public class Buffer<T> : IBufferRebuilder where T : unmanaged
    {
        internal readonly unsafe Buffer* NativePtr;

        public Buffer()
        {
            Length = SynthicEngine.BufferSize;
            var itemSize = UnsafeUtility.SizeOf<T>();
            var itemAlign = UnsafeUtility.AlignOf<T>();
            unsafe
            {
                NativePtr = Lib.create_buffer((nuint)itemSize, (nuint)itemAlign, (nuint)Length);
                if ((UIntPtr)NativePtr == UIntPtr.Zero)
                    throw new SynthicNativeException(
                        $"failed to allocate buffer with size:{itemSize} align:{itemAlign}");
            }

            SynthicEngine.TrackBuffer(this);
        }

        public int Length { get; private set; }

        public ref T this[int index]
        {
            get
            {
                if (index >= Length || index < 0)
                    throw new IndexOutOfRangeException();

                unsafe
                {
                    var bufferPtr = (T*)Lib.buffer_ptr(NativePtr);
                    return ref bufferPtr![index];
                }
            }
        }

        public void RebuildBuffer()
        {
            Length = SynthicEngine.BufferSize;
            unsafe
            {
                Lib.rebuild_buffer(NativePtr, (nuint)Length);
            }
        }

        ~Buffer()
        {
            SynthicEngine.UntrackBuffer(this);
            unsafe
            {
                Lib.dispose_buffer(NativePtr);
            }
        }

        public BufferView<T> NewView()
        {
            return new BufferView<T>(this);
        }

        public void CopyToBuffer(Buffer<T> dstBuffer)
        {
            unsafe
            {
                Lib.copy_to_buffer(NativePtr, dstBuffer.NativePtr);
            }
        }

        public void CopyFromBuffer(Buffer<T> srcBuffer)
        {
            unsafe
            {
                Lib.copy_to_buffer(srcBuffer.NativePtr, NativePtr);
            }
        }

        public void CopyToManaged(T[] dstArray)
        {
            unsafe
            {
                fixed (T* dstPtr = dstArray)
                {
                    Lib.copy_to_ptr(NativePtr, (byte*)dstPtr, (nuint)dstArray.Length);
                }
            }
        }

        public void CopyFromManaged(T[] srcArray)
        {
            unsafe
            {
                fixed (T* srcPtr = srcArray)
                {
                    Lib.copy_from_ptr(NativePtr, (byte*)srcPtr, (nuint)srcArray.Length);
                }
            }
        }

        public void CopyToManagedChannel(T[] dstArray, int totalChannels, int targetChannel)
        {
            if (totalChannels < 1)
                throw new SizeException("totalChannels must be greater than 0");
            if (targetChannel < 0)
                throw new IndexOutOfRangeException("targetChannel must be greater than or equal to 0");
            if (targetChannel >= totalChannels)
                throw new SizeException("targetChannel must be less than totalChannels");

            unsafe
            {
                fixed (T* dstPtr = dstArray)
                {
                    Lib.copy_to_ptr_channel(NativePtr, (byte*)dstPtr, (nuint)dstArray.Length,
                        (nuint)totalChannels, (nuint)targetChannel);
                }
            }
        }

        public void SetAllValues(T value)
        {
            unsafe
            {
                Lib.set_buffer_values(NativePtr, (byte*)&value);
            }
        }

        internal unsafe T* GetBufferPtr()
        {
            return (T*)Lib.buffer_ptr(NativePtr);
        }
    }
}
