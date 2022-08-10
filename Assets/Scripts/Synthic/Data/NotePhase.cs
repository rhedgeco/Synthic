using System.Runtime.InteropServices;

namespace Synthic.Data
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NotePhase
    {
        public bool Active;
        public double Phase;
    }
}