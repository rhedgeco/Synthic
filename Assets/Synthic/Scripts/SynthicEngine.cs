using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Synthic
{
    public static class SynthicEngine
    {
        private static long _currentViewId;
        private static readonly HashSet<IBufferRebuilder> BufferTracker = new();
        private static AudioConfiguration _unityAudioConfig;
        public static int SampleRate => _unityAudioConfig.sampleRate;
        public static int BufferSize => _unityAudioConfig.dspBufferSize;
        public static AudioSpeakerMode SpeakerMode => _unityAudioConfig.speakerMode;

        internal static long CurrentViewId => _currentViewId;

        internal static void TrackBuffer(IBufferRebuilder buffer)
        {
            BufferTracker.Add(buffer);
        }

        internal static void UntrackBuffer(IBufferRebuilder buffer)
        {
            BufferTracker.Remove(buffer);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeAudioSettings()
        {
            AudioSettings.OnAudioConfigurationChanged += ReloadConfig;
            ReloadConfig(false);
            return;

            void ReloadConfig(bool _)
            {
                Interlocked.Add(ref _currentViewId, 1); // invalidate all buffer views
                _unityAudioConfig = AudioSettings.GetConfiguration();
                foreach (var rebuilder in BufferTracker)
                    rebuilder.RebuildBuffer();
            }
        }
    }

    internal interface IBufferRebuilder
    {
        public void RebuildBuffer();
    }
}
