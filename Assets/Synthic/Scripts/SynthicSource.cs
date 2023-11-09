using UnityEngine;

namespace Synthic
{
    [RequireComponent(typeof(AudioSource))]
    public class SynthicSource : MonoBehaviour
    {
        [SerializeField] private InputPort<float>[] inputs;
        private long _sampleTime;

        private void OnAudioFilterRead(float[] data, int channels)
        {
            var sampleTime = _sampleTime;
            _sampleTime += data.Length / channels;
            if (inputs == null) return;
            var channelMin = Mathf.Min(channels, inputs.Length);
            for (var channel = 0; channel < channelMin; channel++)
            {
                var sourceBuffer = inputs[channel].GetSourceBuffer(sampleTime);
                if (sourceBuffer != null) sourceBuffer.CopyToManagedChannel(data, channels, channel);
            }
        }
    }
}
