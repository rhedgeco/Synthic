using Synthic.Native;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace Synthic.Generators
{
    [BurstCompile]
    public class SineGenerator : SynthProvider
    {
        [SerializeField] private int oscillatorResolution = 256;
        [SerializeField] private float frequency = 261.62f; // middle C
        [SerializeField, Range(0, 1)] private float amplitude = 0.5f;

        private int _sampleRate;
        private long _currentSample;
        private Oscillator _oscillator;
        private static BurstSineDelegate _burstSine;

        private void Awake()
        {
            _sampleRate = AudioSettings.GetConfiguration().sampleRate;
            if (_burstSine != null) return;
            _oscillator.Compile(oscillatorResolution, phase => (float) math.sin(phase * 2 * math.PI));
            _burstSine = BurstCompiler.CompileFunctionPointer<BurstSineDelegate>(BurstSine).Invoke;
        }

        protected override void ProcessBuffer(ref SynthBuffer buffer)
        {
            _currentSample = _burstSine(ref buffer, ref _oscillator, _currentSample, _sampleRate, amplitude, frequency);
        }

        private delegate long BurstSineDelegate(ref SynthBuffer buffer, ref Oscillator oscillator,
            long currentSample, int sampleRate, float amplitude, float frequency);

        [BurstCompile]
        private static long BurstSine(ref SynthBuffer buffer, ref Oscillator oscillator,
            long currentSample, int sampleRate, float amplitude, float frequency)
        {
            for (int sample = 0; sample < buffer.Length; sample += buffer.Channels)
            {
                // get total sample progress
                long totalSamples = currentSample + sample / buffer.Channels;
                
                // create a divisor for converting samples to phase
                float sampleFrequency = sampleRate / frequency;

                // convert sample progress into a phase based on frequency
                float phase = totalSamples % sampleFrequency / sampleFrequency;

                // get value of phase on a sine wave
                //float value = math.sin(phase * 2 * math.PI) * amplitude;
                float value = (float) oscillator.Sample(phase) * amplitude;

                for (int channel = 0; channel < buffer.Channels; channel++)
                {
                    buffer[sample + channel] = value;
                }
            }

            return currentSample + buffer.ChannelLength;
        }
    }
}