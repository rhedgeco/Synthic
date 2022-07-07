using Synthic.Native;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace Synthic.Generators
{
    [BurstCompile]
    public class SineGenerator : SynthProvider
    {
        [SerializeField, Range(0, 1)] private float amplitude = 0.5f;
        [SerializeField, Range(16.35f, 7902.13f)] private float frequency = 261.62f; // middle C

        private int _sampleRate;
        private long _currentSample;
        private static BurstSineDelegate _burstSine;

        private void Awake()
        {
            _sampleRate = AudioSettings.GetConfiguration().sampleRate;
            if (_burstSine != null) return;
            _burstSine = BurstCompiler.CompileFunctionPointer<BurstSineDelegate>(BurstSine).Invoke;
        }

        protected override void ProcessBuffer(ref SynthBuffer buffer)
        {
            _currentSample = _burstSine(ref buffer, _currentSample, _sampleRate, amplitude, frequency);
        }

        private delegate long BurstSineDelegate(ref SynthBuffer buffer,
            long currentSample, int sampleRate, float amplitude, float frequency);
    
        [BurstCompile]
        private static unsafe long BurstSine(ref SynthBuffer buffer,
            long currentSample, int sampleRate, float amplitude, float frequency)
        {
            for (int sample = 0; sample < buffer.Length; sample += buffer.Channels)
            {
                // get total sample progress
                long totalSamples = currentSample + sample / buffer.Channels;
            
                // convert sample progress into a phase based on frequency
                float phase = totalSamples * frequency / sampleRate % 1;
            
                // get value of phase on a sine wave
                float value = math.sin(phase * 2 * math.PI) * amplitude;
            
                for (int channel = 0; channel < buffer.Channels; channel++)
                {
                    buffer[sample + channel] = value;
                }
            }

            return currentSample + buffer.ChannelLength;
        }
    }
}