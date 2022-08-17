using System;

namespace Synthic.Native.Core
{
    public unsafe struct BufferRefIterator<T> where T : unmanaged
    {
        private long _bufferPointer;
        private int _bufferLength;
        private long _currentItemPointer;
        private int _currentIndex;

        public int CurrentIndex => _currentIndex;
        public ref T Current => ref *(T*) _currentItemPointer;

        internal BufferRefIterator(ref BufferHandler<T> bufferHandler)
        {
            if (!bufferHandler.Allocated)
                throw new ObjectDisposedException("Cannot create iterator for disposed buffer.");
            _bufferPointer = (long) bufferHandler.Pointer;
            _bufferLength = bufferHandler.Length;
            _currentItemPointer = _bufferPointer - sizeof(T);
            _currentIndex = -1;
        }

        public bool MoveNext()
        {
            _currentItemPointer += sizeof(T);
            _currentIndex++;
            return _currentIndex < _bufferLength;
        }

        public void Reset()
        {
            _currentItemPointer = _bufferPointer - sizeof(T);
            _currentIndex = -1;
        }
    }
}