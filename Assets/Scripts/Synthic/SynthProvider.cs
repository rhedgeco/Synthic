using Synthic.Native;
using UnityEngine;

namespace Synthic
{
    public abstract class SynthProvider : MonoBehaviour
    {
        private NativeDisposer<SynthBuffer> _buffer = new NativeDisposer<SynthBuffer>();

        // processes and fills a managed buffer with sound data
        public void FillBuffer(float[] buffer, int channels)
        {
            EnsureBufferAllocated(buffer.Length, channels);
            ProcessBuffer(ref _buffer.Object);
            _buffer.Object.CopyTo(buffer);
        }
    
        // processes and fills a native buffer with sound data
        public void FillBuffer(ref SynthBuffer buffer)
        {
            if (!buffer.Allocated) return;
            EnsureBufferAllocated(buffer.Length, buffer.Channels);
            ProcessBuffer(ref _buffer.Object);
            _buffer.Object.CopyTo(ref buffer);
        }
    
        // ensures that our cached buffer has the same properties as the incoming buffer
        private void EnsureBufferAllocated(int bufferLength, int channels)
        {
            if (_buffer.Object.Length == bufferLength && _buffer.Object.Channels == channels) return;
            if (_buffer.Object.Allocated) _buffer.Object.Dispose();
            _buffer.Object = new SynthBuffer(bufferLength, channels);
        }

        // override for creating custom providers
        protected abstract void ProcessBuffer(ref SynthBuffer buffer);
    }
}