namespace Synthic.Native.Midi
{
    public struct MidiBuffer : INativeObject
    {
        private BufferHandler<MidiMessage> _buffer;

        public bool Allocated => _buffer.Allocated;

        public static NativeBox<MidiBuffer> Construct(int bufferLength)
        {
            MidiBuffer buffer = new MidiBuffer {_buffer = new BufferHandler<MidiMessage>(bufferLength)};
            return new NativeBox<MidiBuffer>(buffer);
        }

        void INativeObject.ReleaseResources() => _buffer.Dispose();
    }
}