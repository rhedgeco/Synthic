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
                Debug.LogError("Synthic only works with unity STEREO output mode.");
                return;
            }

            if (provider == null) return;
            provider.FillBuffer(data);
        }
    }
}