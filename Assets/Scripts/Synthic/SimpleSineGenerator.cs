using UnityEngine;

namespace Synthic
{
    public class SimpleSineGenerator : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] private float amplitude = 0.5f;
        [SerializeField] private float frequency = 261.62f; // middle C

        private double _phase;
        private int _sampleRate;

        private void Awake()
        {
            _sampleRate = AudioSettings.outputSampleRate;
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            double phaseIncrement = frequency / _sampleRate;

            for (int sample = 0; sample < data.Length; sample += channels)
            {
                float value = Mathf.Sin((float) _phase * 2 * Mathf.PI) * amplitude;

                _phase = (_phase + phaseIncrement) % 1;

                for (int channel = 0; channel < channels; channel++)
                {
                    data[sample + channel] = value;
                }
            }
        }
    }
}
