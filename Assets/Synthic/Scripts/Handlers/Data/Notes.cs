using System;
using System.Runtime.InteropServices;

namespace Synthic.Handlers.Data
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Notes
    {
        public const int NoteCount = 128;

        private unsafe fixed float noteAmplitude[NoteCount];
        private unsafe fixed float noteFrequency[NoteCount];

        public void Set(int index, float amplitude, float frequency)
        {
            if (index is < 0 or >= NoteCount)
                throw new IndexOutOfRangeException();

            unsafe
            {
                noteAmplitude[index] = amplitude;
                noteFrequency[index] = frequency;
            }
        }

        public ref float GetAmplitude(int index)
        {
            if (index is < 0 or >= NoteCount)
                throw new IndexOutOfRangeException();

            unsafe
            {
                return ref noteAmplitude[index];
            }
        }

        public ref float GetFrequency(int index)
        {
            if (index is < 0 or >= NoteCount)
                throw new IndexOutOfRangeException();

            unsafe
            {
                return ref noteFrequency[index];
            }
        }
    }
}
