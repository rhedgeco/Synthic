using System.Runtime.InteropServices;

namespace Synthic.Native.Midi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MidiNote
    {
        private byte _note;

        public byte Velocity { get; private set; }
        public byte NoteIndex => (byte) (_note & 0b_0111_1111);
        public Signal MidiSignal => _note >= 128 ? Signal.On : Signal.Off;

        public static MidiNote On(byte noteIndex, byte velocity)
        {
            return new MidiNote
            {
                _note = (byte) (noteIndex | 0b_1000_0000),
                Velocity = velocity,
            };
        }
        
        public static MidiNote Off(byte noteIndex)
        {
            return new MidiNote
            {
                _note = (byte) (noteIndex & 0b_0111_1111),
                Velocity = 0,
            };
        }

        public enum Signal
        {
            Off,
            On,
        }
    }
}