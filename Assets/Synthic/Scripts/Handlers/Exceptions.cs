using System;

namespace Synthic.Handlers
{
    public class SizeException : Exception
    {
        public SizeException()
        {
        }

        public SizeException(string message) : base(message)
        {
        }
    }

    public class SynthicNativeException : Exception
    {
        public SynthicNativeException()
        {
        }

        public SynthicNativeException(string message) : base(message)
        {
        }
    }

    public class InvalidBufferView : Exception
    {
        public InvalidBufferView()
        {
        }

        public InvalidBufferView(string message) : base(message)
        {
        }
    }
}
