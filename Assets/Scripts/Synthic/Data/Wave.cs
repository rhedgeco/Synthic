using System;
using Unity.Mathematics;

namespace Synthic.Data
{
    public class Wave
    {
        public enum Shape
        {
            Saw,
            Sine,
            Square
        }

        public static float ProcessWave(Shape shape, double phase)
        {
            return shape switch
            {
                Shape.Saw => ProcessSaw(phase),
                Shape.Sine => ProcessSine(phase),
                Shape.Square => ProcessSquare(phase),
                _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, null)
            };
        }

        private static float ProcessSaw(double phase) => (float) (phase % 1 * 2) - 1;
        private static float ProcessSine(double phase) => (float) math.sin(phase * 2 * math.PI);
        private static float ProcessSquare(double phase) => phase < 0.5 ? -1 : 1;
    }
}