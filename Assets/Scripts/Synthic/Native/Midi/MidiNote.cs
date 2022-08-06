namespace Synthic.Native.Midi
{
    public struct MidiNote
    {
        private byte _status;

        public byte Pitch { get; private set; }
        public byte Velocity { get; private set; }
        public bool Status => _status >= 0b_1001_0000;
        public MidiChannel Channel => (MidiChannel) (_status & 0b_0000_1111);

        public static MidiNote On(MidiChannel channel, byte pitch, byte velocity)
        {
            return new MidiNote
            {
                _status = (byte) ((byte) channel | 0b_1001_0000),
                Pitch = (byte) (pitch & 0b_0111_1111),
                Velocity = (byte) (velocity & 0b_0111_1111),
            };
        }
        
        public static MidiNote Off(MidiChannel channel, byte pitch, byte velocity)
        {
            return new MidiNote
            {
                _status = (byte) ((byte) channel | 0b_1000_0000),
                Pitch = (byte) (pitch & 0b_0111_1111),
                Velocity = (byte) (velocity & 0b_0111_1111),
            };
        }
    }
}