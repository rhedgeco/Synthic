use std::f64::consts::PI;

#[repr(i32)]
#[derive(Debug, Default, Clone, Copy, PartialEq, Eq, PartialOrd, Ord, Hash)]
pub enum WaveShape {
    Saw,
    #[default]
    Sine,
    Square,
    Triangle,
}

impl WaveShape {
    pub fn get_value(&self, phase: f64) -> f32 {
        match self {
            WaveShape::Saw => (phase * 2. - 1.) as f32,
            WaveShape::Square => (phase.round() * 2. - 1.) as f32,
            WaveShape::Sine => (phase * 2.0 * PI).sin() as f32,
            WaveShape::Triangle => {
                let offset_phase = phase + 0.25;
                let wave = (offset_phase - offset_phase.round()).abs();
                (wave * 2. - 1.) as f32
            }
        }
    }
}
