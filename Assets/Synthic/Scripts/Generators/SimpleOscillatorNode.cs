using System.Collections.Generic;
using Synthic.Handlers;
using UnityEngine;

namespace Synthic.Generators
{
    public class SimpleOscillatorNode : BufferNode
    {
        [SerializeField] private OscillatorSettings.WaveShape waveShape = OscillatorSettings.WaveShape.Sine;

        [Header("Amplitude")] [SerializeField] [Range(0, 1)]
        private float masterAmplitude = 1;

        [SerializeField] private InputPort<float> amplitudeInput;

        [Header("Frequency")] [SerializeField] [Min(0)]
        private float defaultFrequency = 261.63f;

        [SerializeField] private InputPort<float> frequencyInput;

        [Header("Voicing")] [SerializeField] [Range(1, OscillatorSettings.MaxVoices)]
        private int voices;

        [SerializeField] [Range(0, 1)] private float voicePan = 0.5f;
        [SerializeField] [Range(0, 1)] private float voiceFrequency = 0.1f;
        [SerializeField] [Range(0, 1)] private float voiceAmplitude = 0.5f;

        private readonly Buffer<float> _amplitudeCache = new();
        private readonly Buffer<float> _frequencyCache = new();

        private OscillatorSettings oscSettings;
        private SimpleOscillator simpleOscillator;

        protected override IEnumerable<OutputPort> RegisterOutputs()
        {
            yield return OutputPort.Construct<float>("left");
            yield return OutputPort.Construct<float>("right");
        }

        protected override void ProcessCache(long sampleTime, BufferCache bufferCache)
        {
            oscSettings ??= new OscillatorSettings(waveShape, voices, voicePan, voiceFrequency, voiceAmplitude);
            simpleOscillator ??= new SimpleOscillator(oscSettings, masterAmplitude, defaultFrequency);
            var leftCache = bufferCache.GetChannelBuffer<float>(0);
            var rightCache = bufferCache.GetChannelBuffer<float>(1);
            simpleOscillator.MasterAmplitude = masterAmplitude;
            simpleOscillator.DefaultFrequency = defaultFrequency;
            oscSettings.Shape = waveShape;
            oscSettings.Voices = voices;
            oscSettings.VoicePan = voicePan;
            oscSettings.VoiceAmplitude = voiceAmplitude;
            oscSettings.VoiceFrequency = voiceFrequency;
            simpleOscillator.SetSettings(oscSettings);
            var amplitudeCache = amplitudeInput.GetSourceData(sampleTime, _amplitudeCache) ? _amplitudeCache : null;
            var frequencyCache = frequencyInput.GetSourceData(sampleTime, _frequencyCache) ? _frequencyCache : null;
            simpleOscillator.Process(leftCache, rightCache, amplitudeCache, frequencyCache, SynthicEngine.SampleRate);
        }
    }
}
