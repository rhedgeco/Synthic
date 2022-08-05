using System;

namespace Synthic.Native
{
    public class NativeBox<T> : IDisposable where T : INativeObject
    {
        private T _data;
        public ref T Data => ref _data;
        public bool Allocated => _data.Allocated;

        internal NativeBox(T data)
        {
            _data = data;
        }

        ~NativeBox()
        {
            ReleaseUnmanagedResources();
        }

        private void ReleaseUnmanagedResources()
        {
            if (!_data.Allocated) return;
            _data.ReleaseResources();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }
    }
}