using Unity.Mathematics;

namespace Synthic.Data
{
    public static class Frequency
    {
        public const float A = 440;
        private const float ADiv32 = A / 32f;
        
        public static float ConvertFromMidi(int midiNote)
        {
            return ADiv32 * math.pow(2, (midiNote - 9) / 12f);
        }
    }
}