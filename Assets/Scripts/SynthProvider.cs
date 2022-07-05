using UnityEngine;

public abstract class SynthProvider : MonoBehaviour
{
    private SynthBuffer _buffer;

    // processes and fills a managed buffer with sound data
    public void FillBuffer(float[] buffer, int channels)
    {
        EnsureBufferAllocated(buffer.Length, channels);
        ProcessBuffer(ref _buffer);
        _buffer.Handler.CopyTo(buffer);
    }
    
    // processes and fills a native buffer with sound data
    public void FillBuffer(ref SynthBuffer buffer)
    {
        if (!buffer.Handler.Allocated) return;
        EnsureBufferAllocated(buffer.Handler.Length, buffer.Channels);
        ProcessBuffer(ref _buffer);
        _buffer.Handler.CopyTo(buffer.Handler);
    }
    
    // ensures that our cached buffer has the same properties as the incoming buffer
    private void EnsureBufferAllocated(int bufferLength, int channels)
    {
        if (_buffer.Handler.Length == bufferLength && _buffer.Channels == channels) return;
        if (_buffer.Handler.Allocated) _buffer.Dispose();
        _buffer = new SynthBuffer(bufferLength, channels);
    }

    // override for creating custom providers
    protected abstract void ProcessBuffer(ref SynthBuffer buffer);
}