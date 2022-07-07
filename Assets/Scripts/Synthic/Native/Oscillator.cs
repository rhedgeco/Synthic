using System;
using Unity.Mathematics;

namespace Synthic.Native
{
    public struct Oscillator : IDisposable
    {
        private BufferHandler<float> _waveTable;

        public double Sample(double phase)
        {
            if (!_waveTable.Allocated) return 0;

            double indexDouble = _waveTable.Length * math.clamp(phase, 0, 1);
            float valueFloor = _waveTable[(int) indexDouble];
            float valueCeil = _waveTable[(int) (indexDouble + 0.5)];

            return math.lerp(valueFloor, valueCeil, indexDouble - valueFloor);
        }

        public void Compile(int length, CompileDelegate waveFunction)
        {
            if (!_waveTable.Allocated || _waveTable.Length != length)
            {
                _waveTable.Dispose();
                _waveTable = new BufferHandler<float>(length);
            }

            for (int i = 0; i < length; i++)
            {
                _waveTable[i] = waveFunction((double) i / length);
            }
        }

        public delegate float CompileDelegate(double phase);

        public void Dispose()
        {
            _waveTable.Dispose();
        }
    }
}