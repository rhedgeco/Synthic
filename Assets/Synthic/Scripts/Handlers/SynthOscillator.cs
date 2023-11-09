using System;
using Synthic.Handlers.Data;
using Synthic.Native;

namespace Synthic.Handlers
{
    public class SynthOscillator
    {
        public const byte MinVoices = 1;
        public const byte MaxVoices = 16;

        internal readonly unsafe Native.SynthOscillator* NativePtr;
        private float _masterAmplitude;
        private float _voiceAmplitude;
        private float _voiceFrequency;
        private float _voicePan;
        private int _voices;
        private Oscillator.WaveShapes waveShape;

        public SynthOscillator(int voices, Oscillator.WaveShapes wave, float voicePan,
            float voiceFrequency, float voiceAmplitude, float masterAmplitude)
        {
            SetLocalSettings(voices, wave, voicePan, voiceFrequency, voiceAmplitude, masterAmplitude);

            unsafe
            {
                NativePtr = Lib.create_synth_oscillator((nuint)_voices, (WaveShapes)waveShape, _voicePan,
                    _voiceFrequency, _voiceAmplitude, _masterAmplitude);
                if ((UIntPtr)NativePtr == UIntPtr.Zero)
                    throw new SynthicNativeException("failed to allocate oscillator");
            }
        }

        public Oscillator.WaveShapes Wave
        {
            get => waveShape;
            set
            {
                waveShape = value;
                unsafe
                {
                    Lib.set_synth_oscillator_wave(NativePtr, (WaveShapes)waveShape);
                }
            }
        }

        public float MasterAmplitude
        {
            get => _masterAmplitude;
            set
            {
                _masterAmplitude = value;
                unsafe
                {
                    Lib.set_synth_oscillator_master_amplitude(NativePtr, _masterAmplitude);
                }
            }
        }

        public int Voices
        {
            get => _voices;
            set
            {
                _voices = Math.Clamp(value, MinVoices, MaxVoices);
                unsafe
                {
                    Lib.set_synth_oscillator_voices(NativePtr, (nuint)_voices);
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
                    Lib.set_synth_oscillator_voice_pan(NativePtr, _voicePan);
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
                    Lib.set_synth_oscillator_voice_frequency(NativePtr, _voiceFrequency);
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
                    Lib.set_synth_oscillator_master_amplitude(NativePtr, _voiceAmplitude);
                }
            }
        }

        ~SynthOscillator()
        {
            unsafe
            {
                Lib.dispose_synth_oscillator(NativePtr);
            }
        }

        public void SetSettings(int voices, Oscillator.WaveShapes wave, float voicePan,
            float voiceFrequency, float voiceAmplitude, float masterAmplitude)
        {
            SetLocalSettings(voices, wave, voicePan, voiceFrequency, voiceAmplitude, masterAmplitude);
            unsafe
            {
                Lib.set_synth_oscillator_settings(NativePtr, (nuint)_voices, (WaveShapes)waveShape, _voicePan,
                    _voiceFrequency, _voiceAmplitude, _masterAmplitude);
            }
        }

        private void SetLocalSettings(int voices, Oscillator.WaveShapes wave, float voicePan,
            float voiceFrequency, float voiceAmplitude, float masterAmplitude)
        {
            _voices = Math.Clamp(voices, MinVoices, MaxVoices);
            waveShape = wave;
            _voicePan = voicePan;
            _voiceFrequency = voiceFrequency;
            _voiceAmplitude = voiceAmplitude;
            _masterAmplitude = masterAmplitude;
        }

        public void Process(Buffer<float> leftBuffer, Buffer<float> rightBuffer, Buffer<Notes> notesBuffer,
            int sampleRate)
        {
            if (leftBuffer == null) return;
            if (rightBuffer == null) return;
            if (notesBuffer == null) return;
            unsafe
            {
                Lib.process_synth_oscillator(NativePtr, leftBuffer.NativePtr, rightBuffer.NativePtr,
                    notesBuffer.NativePtr, (nuint)sampleRate);
            }
        }
    }
}
