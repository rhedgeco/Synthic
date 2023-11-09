using Synthic.Native;

namespace Synthic.Handlers.Extensions
{
    public static class BufferExtensions
    {
        public static void MapValues(this Buffer<float> buffer, float inMin, float inMax, float outMin, float outMax)
        {
            unsafe
            {
                Lib.map_buffer_floats(buffer.NativePtr, inMin, inMax, outMin, outMax);
            }
        }
    }
}
