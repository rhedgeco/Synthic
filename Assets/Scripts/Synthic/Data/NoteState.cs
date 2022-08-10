using System.Runtime.InteropServices;
using Synthic.Native.Midi;

namespace Synthic.Data
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NoteState
    {
        public double Time;
        public double ReleaseTime;
        public double Phase;
        public float Velocity;
        public MidiNote.Signal Signal;
    }
}