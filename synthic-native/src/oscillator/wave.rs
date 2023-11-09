use std::f64::consts::PI;

use super::synth::WaveProvider;

#[repr(i32)]
#[derive(Debug, Default, Clone, Copy, PartialEq, Eq, PartialOrd, Ord, Hash)]
pub enum WaveShapes {
    Saw,
    #[default]
    Sine,
    Square,
    Triangle,
}

impl WaveShapes {
    pub fn get_value(&self, phase: f64) -> f32 {
        match self {
            WaveShapes::Saw => (phase * 2. - 1.) as f32,
            WaveShapes::Square => (phase.round() * 2. - 1.) as f32,
            WaveShapes::Sine => (phase * 2.0 * PI).sin() as f32,
            WaveShapes::Triangle => {
                let offset_phase = phase + 0.25;
                let wave = (offset_phase - offset_phase.round()).abs();
                (wave * 2. - 1.) as f32
            }
        }
    }
}

impl WaveProvider for WaveShapes {
    fn get_value(&self, phase: f64) -> f32 {
        self.get_value(phase)
    }
}
