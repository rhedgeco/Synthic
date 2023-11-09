use crate::buffer::TypedBuffer;

use super::WaveShapes;

pub struct Oscillator {
    phase: f64,
    pub mode: WaveShapes,
    pub max_value: f32,
    pub min_value: f32,
    pub master_amplitude: f32,
    pub default_frequency: f32,
}

impl Oscillator {
    pub fn new(
        mode: WaveShapes,
        max_value: f32,
        min_value: f32,
        master_amplitude: f32,
        default_frequency: f32,
    ) -> Self {
        Self {
            phase: 0f64,
            mode,
            max_value,
            min_value,
            master_amplitude,
            default_frequency,
        }
    }

    pub fn process(
        &mut self,
        mut dst_buffer: TypedBuffer<f32>,
        amplitude: Option<TypedBuffer<f32>>,
        frequency: Option<TypedBuffer<f32>>,
        sample_rate: usize,
    ) {
        if self.master_amplitude == 0. {
            return;
        }

        let map_slope = (self.max_value - self.min_value) / 2.;
        for (index, sample) in dst_buffer.iter_mut().enumerate() {
            let amplitude = match &amplitude {
                Some(buffer) => *buffer.get(index).unwrap(),
                None => 1.,
            };

            if amplitude == 0. {
                continue;
            }

            let frequency = match &frequency {
                Some(buffer) => *buffer.get(index).unwrap(),
                None => self.default_frequency,
            };

            let value = self.mode.get_value(self.phase) * amplitude * self.master_amplitude;
            *sample = map_slope * (value + 1.) + self.min_value;
            let phase_delta = frequency as f64 / sample_rate as f64;
            self.phase = (self.phase + phase_delta) % 1f64;
        }
    }
}
