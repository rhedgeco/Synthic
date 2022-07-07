using UnityEngine;

namespace Synthic.Generators
{
    public class SimpleSineGenerator : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] private float amplitude = 0.5f;
        [SerializeField, Range(16.35f, 7902.13f)] private float frequency = 261.62f; // middle C

        private int _sampleRate;
        private long _currentSample;

        private void Awake()
        {
            _sampleRate = AudioSettings.GetConfiguration().sampleRate;
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            for (int sample = 0; sample < data.Length; sample += channels)
            {
                // get total sample progress
                long totalSamples = _currentSample + sample / channels;

                // convert sample progress into a phase based on frequency
                float phase = totalSamples * frequency / _sampleRate % 1;

                // get value of phase on a sine wave
                float value = Mathf.Sin(phase * 2 * Mathf.PI) * amplitude;

                // populate all channels with the values
                for (int channel = 0; channel < channels; channel++)
                {
                    data[sample + channel] = value;
                }
            }

            // increase sample progress for next iteration
            // this needs to be divided by channels the channels variable to account
            // for the fact that all channels are represented in the same buffer
            _currentSample += data.Length / channels;
        }
    }
}