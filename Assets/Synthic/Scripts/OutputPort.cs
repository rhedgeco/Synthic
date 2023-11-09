using System;
using Synthic.Handlers;

namespace Synthic
{
    public class OutputPort
    {
        private object _buffer;

        private OutputPort(string name, Type type, object buffer)
        {
            _buffer = buffer;
            Name = name;
            Type = type;
        }

        public string Name { get; internal set; }
        public Type Type { get; }

        public static OutputPort Construct<T>(string name) where T : unmanaged
        {
            return new OutputPort(name, typeof(T), null);
        }

        internal Buffer<T> GetBuffer<T>() where T : unmanaged
        {
            if (typeof(T) != Type) return null;
            _buffer ??= new Buffer<T>();
            return _buffer as Buffer<T>;
        }
    }
}
