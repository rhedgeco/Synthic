using System;
using Synthic.Native;

namespace Synthic.Handlers
{
    public class OscillatorSettings
    {
        [Serializable]
        public enum WaveShape
        {
            Saw,
            Sine,
            Square,
            Triangle
        }

        public const int MaxVoices = 16;

        internal readonly unsafe Native.OscillatorSettings* NativePtr;
        private float _voiceAmplitude;
        private float _voiceFrequency;
        private float _voicePan;
        private int _voices;
        private WaveShape _waveShape;

        public OscillatorSettings(WaveShape waveShape, int voices, float voicePan, float voiceFrequency,
            float voiceAmplitude)
        {
            _waveShape = waveShape;
            _voices = Math.Clamp(voices, 1, MaxVoices);
            _voicePan = voicePan;
            _voiceFrequency = voiceFrequency;
            _voiceAmplitude = voiceAmplitude;
            unsafe
            {
                NativePtr = Lib.create_oscillator_settings((Native.WaveShape)waveShape, (nuint)voices, voicePan,
                    voiceFrequency, voiceAmplitude);
                if ((UIntPtr)NativePtr == UIntPtr.Zero)
                    throw new SynthicNativeException("failed to allocate oscillator settings");
            }
        }

        public WaveShape Shape
        {
            get => _waveShape;
            set
            {
                _waveShape = value;
                unsafe
                {
                    Lib.set_oscillator_settings_shape(NativePtr, (Native.WaveShape)_waveShape);
                }
            }
        }

        public int Voices
        {
            get => _voices;
            set
            {
                _voices = Math.Clamp(value, 1, MaxVoices);
                unsafe
                {
                    Lib.set_oscillator_settings_voices(NativePtr, (nuint)_voices);
                }
            }
        }

        public float VoicePan
        {
            get => _voicePan;
            set
            {
                _voicePan = value;
                unsafe
                {
                    Lib.set_oscillator_settings_voice_pan(NativePtr, _voicePan);
                }
            }
        }

        public float VoiceFrequency
        {
            get => _voiceFrequency;
            set
            {
                _voiceFrequency = value;
                unsafe
                {
                    Lib.set_oscillator_settings_voice_frequency(NativePtr, _voiceFrequency);
                }
            }
        }

        public float VoiceAmplitude
        {
            get => _voiceAmplitude;
            set
            {
                _voiceAmplitude = value;
                unsafe
                {
                    Lib.set_oscillator_settings_voice_amplitude(NativePtr, _voiceAmplitude);
                }
            }
        }

        ~OscillatorSettings()
        {
            unsafe
            {
                Lib.dispose_oscillator_settings(NativePtr);
            }
        }
    }
}
