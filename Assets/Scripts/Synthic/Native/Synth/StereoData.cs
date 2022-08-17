using System.Runtime.InteropServices;

namespace Synthic.Native.Synth
{
    [StructLayout(LayoutKind.Sequential)]
    public struct StereoData
    {
        public float Left;
        public float Right;

        public void SetBoth(float value)
        {
            Left = value;
            Right = value;
        }
    }
}