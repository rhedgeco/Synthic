use itertools::izip;
use rand::{Rng, SeedableRng};
use rand_xorshift::XorShiftRng;

use crate::buffer::TypedBuffer;

use super::WaveShape;

pub struct OscillatorSettings {
    rng: XorShiftRng,
    voices: usize,
    pub shape: WaveShape,
    pub voice_pan: f32,
    pub voice_frequency: f32,
    pub voice_amplitude: f32,
}

impl Clone for OscillatorSettings {
    fn clone(&self) -> Self {
        Self {
            rng: XorShiftRng::from_entropy(),
            voices: self.voices,
            shape: self.shape,
            voice_pan: self.voice_pan,
            voice_frequency: self.voice_frequency,
            voice_amplitude: self.voice_amplitude,
        }
    }
}

impl OscillatorSettings {
    pub const MAX_VOICES: usize = 16;

    pub fn new(
        shape: WaveShape,
        voices: usize,
        voice_pan: f32,
        voice_frequency: f32,
        voice_amplitude: f32,
    ) -> Self {
        Self {
            rng: XorShiftRng::from_entropy(),
            shape,
            voices: voices.clamp(1, Self::MAX_VOICES),
            voice_pan,
            voice_frequency,
            voice_amplitude,
        }
    }

    pub fn voices(&self) -> usize {
        self.voices
    }

    pub fn set_voices(&mut self, count: usize) {
        self.voices = count.clamp(1, Self::MAX_VOICES)
    }
}

pub struct OscillatorCore {
    playing: bool,
    phases: [f64; OscillatorSettings::MAX_VOICES],
}

impl Default for OscillatorCore {
    fn default() -> Self {
        Self {
            playing: false,
            phases: [0.; OscillatorSettings::MAX_VOICES],
        }
    }
}

impl OscillatorCore {
    pub fn new() -> Self {
        Self::default()
    }

    pub fn sample(
        &mut self,
        amplitude: f32,
        frequency: f64,
        sample_rate: f64,
        settings: &mut OscillatorSettings,
    ) -> (f32, f32) {
        // do nothing if amplitude is zero
        if amplitude == 0. {
            self.playing = false;
            return (0., 0.);
        }

        // reset phases when a note is first pressed
        if self.playing == false {
            self.playing = true;
            for phase in self.phases.iter_mut() {
                *phase = settings.rng.gen_range(0f64..1f64);
            }
        }

        // use simple calculation with 1 voice
        let total_voices = settings.voices();
        if total_voices == 1 {
            let phase = &mut self.phases[0];
            let sample_value = settings.shape.get_value(*phase);
            *phase = (*phase + (frequency / sample_rate)) % 1.;
            return (sample_value, sample_value);
        }

        // calculate unison voices
        // https://www.desmos.com/calculator/5dxkp1d1a5
        let mut left_sample = 0.;
        let mut right_sample = 0.;
        let low_center = (total_voices as f32 / 2. - 0.25) as usize;
        let high_center = (total_voices as f32 / 2. + 0.25) as usize;
        for voice in 0..total_voices {
            let voice_amp = match voice {
                v if low_center <= v && v <= high_center => 1.,
                _ => settings.voice_amplitude,
            };

            let voice_offset = (2. * voice as f64) / (total_voices as f64 - 1.) - 1.;
            let frequency_offset = 2f64.powf(voice_offset * settings.voice_frequency as f64 / 12.);
            let voice_frequency = frequency * frequency_offset;

            let phase = &mut self.phases[voice];
            let voice_sample = settings.shape.get_value(*phase) * voice_amp * amplitude;
            *phase = (*phase + (voice_frequency / sample_rate)) % 1.;

            let voice_pan = settings.voice_pan * voice_offset as f32;
            let left_pan_amp = 1. - voice_pan.clamp(0., 1.);
            let right_pan_amp = 1. - voice_pan.clamp(-1., 0.).abs();
            left_sample += voice_sample * left_pan_amp;
            right_sample += voice_sample * right_pan_amp;
        }

        (left_sample, right_sample)
    }
}

pub struct SimpleOscillator {
    core: OscillatorCore,
    pub settings: OscillatorSettings,
    pub master_amplitude: f32,
    pub default_frequency: f32,
}

impl SimpleOscillator {
    pub fn new(
        settings: OscillatorSettings,
        master_amplitude: f32,
        default_frequency: f32,
    ) -> Self {
        Self {
            core: OscillatorCore::default(),
            settings,
            master_amplitude,
            default_frequency,
        }
    }

    pub fn process_buffer(
        &mut self,
        left_samples: TypedBuffer<f32>,
        right_samples: TypedBuffer<f32>,
        amplitude: Option<TypedBuffer<f32>>,
        frequency: Option<TypedBuffer<f32>>,
        sample_rate: f64,
    ) {
        for (i, (left_sample, right_sample)) in izip!(left_samples, right_samples).enumerate() {
            let amplitude = match &amplitude {
                Some(buffer) => buffer.get(i).map_or(1., |v| *v),
                _ => 1.,
            };

            let frequency = match &frequency {
                Some(buffer) => buffer.get(i).map_or(self.default_frequency, |v| *v),
                _ => self.default_frequency,
            };

            let (left_value, right_value) =
                self.core
                    .sample(amplitude, frequency as f64, sample_rate, &mut self.settings);
            *left_sample = left_value * self.master_amplitude;
            *right_sample = right_value * self.master_amplitude;
        }
    }
}
