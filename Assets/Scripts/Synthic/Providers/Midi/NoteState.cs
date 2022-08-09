using Synthic.Native.Midi;

namespace Synthic.Providers.Midi
{
    public struct NoteState
    {
        public MidiNote.Signal Signal;
        public double Phase;
        public byte Velocity;
        public float VelocityFloat => Velocity / (float) byte.MaxValue;

        public NoteState(MidiNote.Signal signal = MidiNote.Signal.On, double phase = 0, byte velocity = 0)
        {
            Signal = signal;
            Phase = phase;
            Velocity = velocity;
        }
    }
}