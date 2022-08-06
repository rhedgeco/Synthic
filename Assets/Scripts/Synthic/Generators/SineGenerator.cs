using Synthic.Native;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace Synthic
{
    [BurstCompile]
    public class SineGenerator : SynthProvider
    {
        [SerializeField, Range(0, 1)] private float amplitude = 0.5f;
        [SerializeField, Range(16.35f, 7902.13f)] private float frequency = 261.62f; // middle C
        
        private static BurstSineDelegate _burstSine;
    
        private double _phase;
        private int _sampleRate;
    
        private void Awake()
        {
            _sampleRate = AudioSettings.GetConfiguration().sampleRate;
            if (_burstSine != null) return;
            _burstSine = BurstCompiler.CompileFunctionPointer<BurstSineDelegate>(BurstSine).Invoke;
        }
        
        protected override void ProcessBuffer(ref SynthBuffer buffer)
        {
            _phase = _burstSine(ref buffer, _phase, _sampleRate, amplitude, frequency);
        }
        
        private delegate double BurstSineDelegate(ref SynthBuffer buffer,
            double phase, int sampleRate, float amplitude, float frequency);
        
        [BurstCompile]
        private static double BurstSine(ref SynthBuffer buffer,
            double phase, int sampleRate, float amplitude, float frequency)
        {
            // calculate how much the phase should change after each sample
            double phaseIncrement = frequency / sampleRate;

            for (int sample = 0; sample < buffer.Length; sample += buffer.Channels)
            {
                // get value of phase on a sine wave
                float value = (float) (math.sin(phase * 2 * math.PI) * amplitude);
        
                // increment _phase value for next iteration
                phase = (phase + phaseIncrement) % 1;
    
                // populate all channels with the values
                for (int channel = 0; channel < buffer.Channels; channel++)
                {
                    buffer[sample + channel] = value;
                }
            }
    
            // return the updated phase
            return phase;
        }
    }
}