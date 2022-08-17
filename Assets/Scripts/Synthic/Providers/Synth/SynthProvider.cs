using Synthic.Native;
using Synthic.Native.Core;
using UnityEngine;

namespace Synthic.Providers.Synth
{
    public abstract class SynthProvider : MonoBehaviour
    {
        private NativeBox<SynthBuffer> _buffer;

        public void FillBuffer(float[] buffer, int channels)
        {
            ValidateBuffer(buffer.Length, channels);
            ProcessBuffer(ref _buffer.Data);
            _buffer.Data.CopyTo(buffer);
        }
        
        // processes and fills a native buffer with sound data
        public void FillBuffer(ref SynthBuffer buffer)
        {
            if (!buffer.Allocated) return;
            ValidateBuffer(buffer.Length, buffer.Channels);
            ProcessBuffer(ref _buffer.Data);
            _buffer.Data.CopyTo(ref buffer);
        }
        
        private void ValidateBuffer(int bufferLength, int channels)
        {
            if (!(_buffer is {Allocated: true})) 
                _buffer = SynthBuffer.Construct(bufferLength, channels);

            ref SynthBuffer buffer = ref _buffer.Data;
            if (buffer.Length == bufferLength && buffer.Channels == channels) return;
            if (buffer.Allocated) _buffer.Dispose();
            _buffer = SynthBuffer.Construct(bufferLength, channels);
        }
        
        // override for creating custom providers
        protected abstract void ProcessBuffer(ref SynthBuffer buffer);
    }
}