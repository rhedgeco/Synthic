using System;
using Synthic.Native;
using Buffer = Synthic.Native.Buffer;

namespace Synthic.Handlers
{
    public class Oscillator
    {
        [Serializable]
        public enum WaveShapes
        {
            Saw = 0,
            Sine = 1,
            Square = 2,
            Triangle = 3
        }

        internal readonly unsafe Native.Oscillator* NativePtr;
        private float _defaultFrequency;
        private float _masterAmplitude;
        private float _maxValue;
        private float _minValue;
        private WaveShapes _waveShape;

        public Oscillator(WaveShapes waveShape, float maxValue, float minValue,
            float masterAmplitude, float defaultFrequency)
        {
            _waveShape = waveShape;
            _maxValue = maxValue;
            _minValue = minValue;
            _masterAmplitude = masterAmplitude;
            _defaultFrequency = defaultFrequency;

            unsafe
            {
                NativePtr = Lib.create_oscillator((Native.WaveShapes)waveShape, maxValue, minValue,
                    masterAmplitude, defaultFrequency);
                if ((UIntPtr)NativePtr == UIntPtr.Zero)
                    throw new SynthicNativeException("failed to allocate oscillator");
            }
        }

        public WaveShapes Mode
        {
            get => _waveShape;
            set
            {
                _waveShape = value;
                unsafe
                {
                    Lib.set_oscillator_mode(NativePtr, (Native.WaveShapes)_waveShape);
                }
            }
        }

        public float MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                unsafe
                {
                    Lib.set_oscillator_max_value(NativePtr, _maxValue);
                }
            }
        }

        public float MinValue
        {
            get => _minValue;
            set
            {
                _minValue = value;
                unsafe
                {
                    Lib.set_oscillator_min_value(NativePtr, _minValue);
                }
            }
        }

        public float MasterAmplitude
        {
            get => _masterAmplitude;
            set
            {
                _masterAmplitude = value;
                unsafe
                {
                    Lib.set_oscillator_master_amplitude(NativePtr, _masterAmplitude);
                }
            }
        }

        public float DefaultFrequency
        {
            get => _defaultFrequency;
            set
            {
                _defaultFrequency = value;
                unsafe
                {
                    Lib.set_oscillator_default_frequency(NativePtr, _defaultFrequency);
                }
            }
        }

        ~Oscillator()
        {
            unsafe
            {
                Lib.dispose_oscillator(NativePtr);
            }
        }

        public void SetSettings(WaveShapes waveShape, float maxValue, float minValue,
            float masterAmplitude, float defaultFrequency)
        {
            _waveShape = waveShape;
            _maxValue = maxValue;
            _minValue = minValue;
            _masterAmplitude = masterAmplitude;
            _defaultFrequency = defaultFrequency;
            unsafe
            {
                Lib.set_oscillator_settings(NativePtr, (Native.WaveShapes)waveShape, maxValue, minValue,
                    masterAmplitude, defaultFrequency);
            }
        }

        public void Process(Buffer<float> dstBuffer, Buffer<float> amplitude, Buffer<float> frequency,
            int sampleRate)
        {
            unsafe
            {
                if (dstBuffer == null) return;
                var amplitudePtr = amplitude == null ? (Buffer*)UIntPtr.Zero : amplitude.NativePtr;
                var frequencyPtr = frequency == null ? (Buffer*)UIntPtr.Zero : frequency.NativePtr;
                Lib.process_oscillator(NativePtr, dstBuffer.NativePtr, amplitudePtr, frequencyPtr,
                    (nuint)sampleRate);
            }
        }
    }
}
