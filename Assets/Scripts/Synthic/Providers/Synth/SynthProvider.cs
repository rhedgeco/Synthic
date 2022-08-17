using Synthic.Native;
using Synthic.Native.Core;
using Synthic.Native.Synth;
using UnityEngine;

namespace Synthic.Providers.Synth
{
    public abstract class SynthProvider : MonoBehaviour
    {
        private NativeBox<SynthBuffer> _buffer;

        public void FillBuffer(float[] buffer)
        {
            ValidateBuffer(buffer.Length);
            ProcessBuffer(ref _buffer.Data);
            _buffer.Data.CopyTo(buffer);
        }
        
        // processes and fills a native buffer with sound data
        public void FillBuffer(ref SynthBuffer buffer)
        {
            if (!buffer.Allocated) return;
            ValidateBuffer(buffer.Length);
            ProcessBuffer(ref _buffer.Data);
            _buffer.Data.CopyTo(ref buffer);
        }
        
        private void ValidateBuffer(int bufferLength)
        {
            if (!(_buffer is {Allocated: true})) 
                _buffer = SynthBuffer.Construct(bufferLength);

            ref SynthBuffer buffer = ref _buffer.Data;
            if (buffer.Length == bufferLength) return;
            if (buffer.Allocated) _buffer.Dispose();
            _buffer = SynthBuffer.Construct(bufferLength);
        }
        
        // override for creating custom providers
        protected abstract void ProcessBuffer(ref SynthBuffer buffer);
    }
}