using System.Collections.Generic;
using Synthic.Handlers;
using UnityEngine;

namespace Synthic.Generators
{
    public class OscillatorNode : BufferNode
    {
        [SerializeField] private float waveMax = 1;
        [SerializeField] private float waveMin = -1;

        [Header("Amplitude")] [SerializeField] [Range(0, 1)]
        private float masterAmplitude = 1;

        [SerializeField] private InputPort<float> amplitudeInput;

        [Header("Frequency")] [SerializeField] [Min(0)]
        private float defaultFrequency = 261.63f;

        [SerializeField] private InputPort<float> frequencyInput;

        private readonly Buffer<float> _amplitudeCache = new();
        private readonly Buffer<float> _frequencyCache = new();
        private readonly Oscillator _oscillator = new(Oscillator.WaveShapes.Sine, 1, -1, 1, 261.63f);

        [Header("Wave Shape")] [SerializeField]
        private readonly Oscillator.WaveShapes waveType = Oscillator.WaveShapes.Sine;

        protected override IEnumerable<OutputPort> RegisterOutputs()
        {
            yield return OutputPort.Construct<float>("osc");
        }

        protected override void ProcessCache(long sampleTime, BufferCache bufferCache)
        {
            var oscCache = bufferCache.GetChannelBuffer<float>(0);
            _oscillator.SetSettings(waveType, waveMax, waveMin, masterAmplitude, defaultFrequency);
            var amplitudeCache = amplitudeInput.GetSourceData(sampleTime, _amplitudeCache) ? _amplitudeCache : null;
            var frequencyCache = frequencyInput.GetSourceData(sampleTime, _frequencyCache) ? _frequencyCache : null;
            _oscillator.Process(oscCache, amplitudeCache, frequencyCache, SynthicEngine.SampleRate);
        }
    }
}
