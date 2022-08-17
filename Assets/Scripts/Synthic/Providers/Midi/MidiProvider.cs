using Synthic.Native;
using Synthic.Native.Core;
using Synthic.Native.Midi;
using UnityEngine;

namespace Synthic.Providers.Midi
{
    public abstract class MidiProvider : MonoBehaviour
    {
        private NativeBox<MidiBuffer> _buffer;

        // processes and fills a native buffer with sound data
        public ref MidiBuffer ReadBuffer(int bufferLength)
        {
            ValidateBuffer(bufferLength);
            ProcessBuffer(ref _buffer.Data);
            return ref _buffer.Data;
        }
        
        private void ValidateBuffer(int bufferLength)
        {
            if (_buffer == null)
            {
                _buffer = MidiBuffer.Construct(bufferLength);
                return;
            }

            ref MidiBuffer buffer = ref _buffer.Data;
            if (buffer.Length == bufferLength) return;
            if (buffer.Allocated) _buffer.Dispose();
            _buffer = MidiBuffer.Construct(bufferLength);
        }
        
        // override for creating custom providers
        protected abstract void ProcessBuffer(ref MidiBuffer buffer);
    }
}