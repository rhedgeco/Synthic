using System;
using Synthic.Native;
using Buffer = Synthic.Native.Buffer;

namespace Synthic.Handlers
{
    public class SimpleOscillator
    {
        internal readonly unsafe Native.SimpleOscillator* NativePtr;
        private float _defaultFrequency;
        private float _masterAmplitude;

        public SimpleOscillator(OscillatorSettings copySettings, float masterAmplitude, float defaultFrequency)
        {
            _masterAmplitude = masterAmplitude;
            _defaultFrequency = defaultFrequency;

            unsafe
            {
                NativePtr = Lib.create_simple_oscillator(copySettings.NativePtr, masterAmplitude, defaultFrequency);
                if ((UIntPtr)NativePtr == UIntPtr.Zero)
                    throw new SynthicNativeException("failed to allocate oscillator");
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
                    Lib.set_simple_oscillator_master_amplitude(NativePtr, _masterAmplitude);
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
                    Lib.set_simple_oscillator_default_frequency(NativePtr, _defaultFrequency);
                }
            }
        }

        ~SimpleOscillator()
        {
            unsafe
            {
                Lib.dispose_simple_oscillator(NativePtr);
            }
        }

        public void SetSettings(OscillatorSettings copySettings)
        {
            unsafe
            {
                Lib.set_simple_oscillator_settings(NativePtr, copySettings.NativePtr);
            }
        }

        public void Process(Buffer<float> leftSamples, Buffer<float> rightSamples,
            Buffer<float> amplitude, Buffer<float> frequency, int sampleRate)
        {
            unsafe
            {
                if (leftSamples == null) return;
                if (rightSamples == null) return;
                var amplitudePtr = amplitude == null ? (Buffer*)UIntPtr.Zero : amplitude.NativePtr;
                var frequencyPtr = frequency == null ? (Buffer*)UIntPtr.Zero : frequency.NativePtr;
                Lib.process_simple_oscillator(NativePtr, leftSamples.NativePtr, rightSamples.NativePtr,
                    amplitudePtr, frequencyPtr, sampleRate);
            }
        }
    }
}
