using System.Runtime.InteropServices;

namespace Synthic.Native.Data
{
    [StructLayout(LayoutKind.Sequential)]
    public struct StereoData
    {
        public float LeftChannel;
        public float RightChannel;

        public StereoData(float value)
        {
            LeftChannel = value;
            RightChannel = value;
        }
    }
}