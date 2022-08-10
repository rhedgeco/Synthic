using Unity.Mathematics;

namespace Synthic.Data
{
    public static class Frequency
    {
        public const float A = 440;
        
        public static float ConvertFromMidi(int midiNote)
        {
            return A / 32 * math.pow(2, (midiNote - 9) / 12f);
        }
    }
}