using Synthic.Providers.Synth;
using UnityEngine;

namespace Synthic
{
    public class SynthOut : MonoBehaviour
    {
        [SerializeField] private SynthProvider provider;

        private void OnAudioFilterRead(float[] data, int channels)
        {
            provider.FillBuffer(data, channels);
        }
    }
}