using System;
using Unity.Mathematics;

namespace Synthic.Native
{
    public struct Oscillator : IDisposable
    {
        private BufferHandler<float> _waveBuffer;

        public double Sample(double phase)
        {
            if (!_waveBuffer.Allocated) return 0;
            if (phase < 0) return _waveBuffer[0];
            if (phase >= 1) return _waveBuffer[_waveBuffer.Length - 1];

            double indexDouble = _waveBuffer.Length * math.clamp(phase, 0, 1);
            int indexFloor = (int) indexDouble;
            int indexCeil = indexFloor + 1;
            float valueFloor = _waveBuffer[indexFloor];
            float valueCeil = _waveBuffer[indexCeil == _waveBuffer.Length ? 0 : indexCeil ];

            return math.lerp(valueFloor, valueCeil, indexDouble - indexFloor);
        }

        public void Compile(int length, CompileDelegate waveFunction)
        {
            if (!_waveBuffer.Allocated || _waveBuffer.Length != length)
            {
                _waveBuffer.Dispose();
                _waveBuffer = new BufferHandler<float>(length);
            }

            for (int i = 0; i < length; i++)
            {
                _waveBuffer[i] = waveFunction((double) i / length);
            }
        }

        public delegate float CompileDelegate(double phase);

        public void Dispose()
        {
            _waveBuffer.Dispose();
        }
    }
}