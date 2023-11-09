use itertools::izip;
use rand::{Rng, SeedableRng};
use rand_xorshift::XorShiftRng;

use super::WaveShapes;
use crate::buffer::TypedBuffer;
use crate::Notes;

pub struct SynthOscillator {
    cores: [OscCore; Notes::NOTE_COUNT],
    pub settings: OscSettings<WaveShapes>,
    pub master_amplitude: f32,
}

impl SynthOscillator {
    pub const SEMITONE_HZ: f32 = 1.059463094;
    pub const MIN_VOICES: usize = 1;
    pub const MAX_VOICES: usize = 16;

    pub fn new(settings: OscSettings<WaveShapes>, master_amplitude: f32) -> Self {
        Self {
            cores: [OscCore::default(); Notes::NOTE_COUNT],
            settings,
            master_amplitude,
        }
    }

    pub fn process(
        &mut self,
        left_buffer: TypedBuffer<f32>,
        right_buffer: TypedBuffer<f32>,
        notes: TypedBuffer<Notes>,
        sample_rate: usize,
    ) {
        let sample_rate = sample_rate as f64;
        for (notes, left_sample, right_sample) in izip!(&notes, left_buffer, right_buffer) {
            // initialize samples to a zero state
            *left_sample = 0.;
            *right_sample = 0.;

            // calculate the values for every note
            for (core, note) in izip!(&mut self.cores, notes) {
                let (left_value, right_value) = core.sample(
                    &mut self.settings,
                    note.amplitude(),
                    note.frequency(),
                    sample_rate,
                );
                *left_sample += left_value;
                *right_sample += right_value;
            }

            *left_sample *= self.master_amplitude;
            *right_sample *= self.master_amplitude;
        }
    }
}

pub trait WaveProvider {
    fn get_value(&self, phase: f64) -> f32;
}

pub struct OscSettings<W: WaveProvider> {
    rng: XorShiftRng,
    voices: usize,
    pub wave: W,
    pub voice_pan: f32,
    pub voice_frequency: f32,
    pub voice_amplitude: f32,
}

impl<W: WaveProvider + Default> Default for OscSettings<W> {
    fn default() -> Self {
        Self {
            rng: XorShiftRng::from_entropy(),
            wave: Default::default(),
            voices: Default::default(),
            voice_pan: Default::default(),
            voice_frequency: Default::default(),
            voice_amplitude: Default::default(),
        }
    }
}

impl<W: WaveProvider> OscSettings<W> {
    pub fn new(
        voices: usize,
        wave: W,
        voice_pan: f32,
        voice_frequency: f32,
        voice_amplitude: f32,
    ) -> Self {
        Self {
            rng: XorShiftRng::from_entropy(),
            voices: voices.clamp(OscCore::MIN_VOICES, OscCore::MAX_VOICES),
            wave,
            voice_pan,
            voice_frequency,
            voice_amplitude,
        }
    }

    pub fn voices(&self) -> usize {
        self.voices
    }

    pub fn set_voices(&mut self, value: usize) {
        self.voices = value.clamp(OscCore::MIN_VOICES, OscCore::MAX_VOICES);
    }
}

#[derive(Debug, Default, Clone, Copy)]
pub struct OscCore {
    playing: bool,
    phases: [f64; Self::MAX_VOICES],
}

impl OscCore {
    pub const MIN_VOICES: usize = 1;
    pub const MAX_VOICES: usize = 16;

    pub fn new() -> Self {
        Self::default()
    }

    pub fn sample<W: WaveProvider>(
        &mut self,
        settings: &mut OscSettings<W>,
        amplitude: f32,
        frequency: f32,
        sample_rate: f64,
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
        let frequency = frequency as f64;
        if total_voices == 1 {
            let phase = &mut self.phases[0];
            let sample_value = settings.wave.get_value(*phase);
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
            let voice_sample = settings.wave.get_value(*phase) * voice_amp * amplitude;
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
