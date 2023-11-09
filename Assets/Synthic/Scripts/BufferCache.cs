using System;
using System.Collections.Generic;
using Synthic.Handlers;

namespace Synthic
{
    public class BufferCache
    {
        private readonly List<OutputPort> _buffers = new();

        public BufferCache(IEnumerable<OutputPort> ports)
        {
            _buffers = new List<OutputPort>();
            foreach (var port in ports)
            {
                port.Name ??= $"port{_buffers.Count}";
                _buffers.Add(port);
            }
        }

        public int Channels => _buffers.Count;

        public string GetChannelName(int index)
        {
            return _buffers[index].Name;
        }

        public Type GetChannelTypeName(int index)
        {
            return _buffers[index].Type;
        }

        public Buffer<T> GetChannelBuffer<T>(int index) where T : unmanaged
        {
            return _buffers[index].GetBuffer<T>();
        }
    }
}
