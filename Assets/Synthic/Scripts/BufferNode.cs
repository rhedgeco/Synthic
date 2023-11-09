using System;
using System.Collections.Generic;
using Synthic.Handlers;
using UnityEngine;

namespace Synthic
{
    public abstract class BufferNode : MonoBehaviour
    {
        [NonSerialized] private BufferCache _bufferCache;
        [NonSerialized] private long _lastSampleTime = -1;

        public IEnumerable<(string, Type)> GetOutputsInfo()
        {
            _bufferCache ??= new BufferCache(RegisterOutputs());
            for (var channel = 0; channel < _bufferCache.Channels; channel++)
                yield return (_bufferCache.GetChannelName(channel), _bufferCache.GetChannelTypeName(channel));
        }

        public (string, Type) GetOutputInfo(int channel)
        {
            if (channel > _bufferCache.Channels) return (null, null);
            _bufferCache ??= new BufferCache(RegisterOutputs());
            return (_bufferCache.GetChannelName(channel), _bufferCache.GetChannelTypeName(channel));
        }

        public void GetOutputData<T>(long sampleTime, int channel, Buffer<T> buffer) where T : unmanaged
        {
            var targetBuffer = GetOutputBuffer<T>(sampleTime, channel);
            if (targetBuffer == null) buffer.SetAllValues(new T());
            else targetBuffer.CopyToBuffer(buffer);
        }

        internal Buffer<T> GetOutputBuffer<T>(long sampleTime, int channel) where T : unmanaged
        {
            _bufferCache ??= new BufferCache(RegisterOutputs());
            if (sampleTime != _lastSampleTime)
            {
                ProcessCache(sampleTime, _bufferCache);
                _lastSampleTime = sampleTime;
            }

            return _bufferCache.GetChannelBuffer<T>(channel);
        }

        protected abstract IEnumerable<OutputPort> RegisterOutputs();

        protected abstract void ProcessCache(long sampleTime, BufferCache bufferCache);
    }
}
