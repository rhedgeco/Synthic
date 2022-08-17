using Synthic.Providers.Synth;
using UnityEngine;

namespace Synthic
{
    public class SynthOut : MonoBehaviour
    {
        [SerializeField] private SynthProvider provider;

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (channels != 2)
            {
                Debug.LogError("Synthic only supports STEREO output. Please switch your audio setting to STEREO.");
                return;
            }
            
            provider.FillBuffer(data);
        }
    }
}