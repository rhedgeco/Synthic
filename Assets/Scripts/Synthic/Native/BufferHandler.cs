using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Synthic.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct BufferHandler<T> : IDisposable where T : unmanaged
    {
        public int Length { get; private set; }
        public T* Pointer { get; private set; }
        public bool Allocated => (IntPtr) Pointer != IntPtr.Zero;

        public BufferHandler(int length)
        {
            Length = length;
            Pointer = (T*) UnsafeUtility.Malloc(Length * sizeof(T), UnsafeUtility.AlignOf<T>(), Allocator.Persistent);
            Clear();
        }

        public BufferHandler(T[] managedArray)
        {
            if (managedArray == null) throw new NullReferenceException("Cannot copy. Managed array is null");
            Length = managedArray.Length;
            Pointer = (T*) UnsafeUtility.Malloc(Length * sizeof(T), UnsafeUtility.AlignOf<T>(), Allocator.Persistent);
            GCHandle gcHandle = GCHandle.Alloc(managedArray, GCHandleType.Pinned);
            UnsafeUtility.MemCpy(Pointer, (T*) gcHandle.AddrOfPinnedObject(), Length * sizeof(T));
            gcHandle.Free();
        }

        public void Dispose()
        {
            if (!Allocated) return;
            UnsafeUtility.Free(Pointer, Allocator.Persistent);
            Pointer = (T*) IntPtr.Zero;
        }

        public void CopyTo(T[] managedArray)
        {
            if (managedArray == null) throw new NullReferenceException("Cannot copy. Managed array is null");
            if (!Allocated) throw new ObjectDisposedException("Cannot copy. Buffer has been disposed");
            int length = Math.Min(managedArray.Length, Length);
            GCHandle gcHandle = GCHandle.Alloc(managedArray, GCHandleType.Pinned);
            UnsafeUtility.MemCpy((T*) gcHandle.AddrOfPinnedObject(), Pointer, length * sizeof(T));
            gcHandle.Free();
        }

        public void CopyTo(BufferHandler<T> buffer)
        {
            if (!Allocated) throw new ObjectDisposedException("Cannot copy. Source buffer has been disposed");
            if (!buffer.Allocated) throw new ObjectDisposedException("Cannot copy. Dest buffer has been disposed");
            int length = Math.Min(Length, buffer.Length);
            UnsafeUtility.MemCpy(Pointer, buffer.Pointer, length * sizeof(T));
        }

        // use pointers to access and set the data in the buffer
        public ref T this[int index]
        {
            get
            {
                CheckAndThrow(index);
                return ref *(T*) ((long) Pointer + index * sizeof(T));
            }
        }

        // utility method to validate an index in the buffer
        private void CheckAndThrow(int index)
        {
            if (!Allocated) throw new ObjectDisposedException("Buffer is disposed");
            if (index >= Length || index < 0)
                throw new IndexOutOfRangeException($"index:{index} out of range:0-{Length}");
        }

        public void Clear()
        {
            UnsafeUtility.MemClear(Pointer, (long) Length * UnsafeUtility.SizeOf<T>());
        }
    }
}