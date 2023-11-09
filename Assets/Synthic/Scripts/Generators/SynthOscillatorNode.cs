using System.Collections.Generic;
using Synthic.Handlers;
using Synthic.Handlers.Data;
using UnityEngine;

namespace Synthic.Generators
{
    public class SynthOscillatorNode : BufferNode
    {
        [SerializeField] [Range(0, 1)] private float masterAmplitude = 0.25f;

        [SerializeField] [Range(SynthOscillator.MinVoices, SynthOscillator.MaxVoices)]
        private byte voices = 7;

        [SerializeField] [Range(0, 1)] private float voicePan = 0.5f;
        [SerializeField] [Min(0)] private float voiceFrequency = 0.1f;
        [SerializeField] [Range(0, 1)] private float voiceAmplitude = 0.5f;

        [SerializeField] private InputPort<Notes> notesInput;

        private readonly Buffer<Notes> _notesCache = new();
        private readonly SynthOscillator _oscillator = new(7, Oscillator.WaveShapes.Saw, 0.5f, 0.1f, 0.5f, 0.25f);
        [SerializeField] private readonly Oscillator.WaveShapes waveType = Oscillator.WaveShapes.Saw;

        protected override IEnumerable<OutputPort> RegisterOutputs()
        {
            yield return OutputPort.Construct<float>("left");
            yield return OutputPort.Construct<float>("right");
        }

        protected override void ProcessCache(long sampleTime, BufferCache bufferCache)
        {
            var leftCache = bufferCache.GetChannelBuffer<float>(0);
            var rightCache = bufferCache.GetChannelBuffer<float>(1);
            _oscillator.SetSettings(voices, waveType, voicePan, voiceFrequency, voiceAmplitude, masterAmplitude);
            var notesCache = notesInput.GetSourceData(sampleTime, _notesCache) ? _notesCache : null;
            _oscillator.Process(leftCache, rightCache, notesCache, SynthicEngine.SampleRate);
        }
    }
}
