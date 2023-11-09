use crate::{Buffer, Oscillator};

use super::WaveShapes;

#[no_mangle]
unsafe extern "C" fn create_oscillator(
    mode: WaveShapes,
    max_value: f32,
    min_value: f32,
    default_amplitude: f32,
    default_frequency: f32,
) -> *mut Oscillator {
    Box::into_raw(Box::new(Oscillator::new(
        mode,
        max_value,
        min_value,
        default_amplitude,
        default_frequency,
    )))
}

#[no_mangle]
unsafe extern "C" fn dispose_oscillator(oscillator: *mut Oscillator) {
    drop(Box::from_raw(oscillator))
}

#[no_mangle]
unsafe extern "C" fn set_oscillator_mode(oscillator: *mut Oscillator, mode: WaveShapes) {
    (&mut *oscillator).mode = mode;
}

#[no_mangle]
unsafe extern "C" fn set_oscillator_max_value(oscillator: *mut Oscillator, value: f32) {
    (&mut *oscillator).max_value = value;
}

#[no_mangle]
unsafe extern "C" fn set_oscillator_min_value(oscillator: *mut Oscillator, value: f32) {
    (&mut *oscillator).min_value = value;
}

#[no_mangle]
unsafe extern "C" fn set_oscillator_master_amplitude(oscillator: *mut Oscillator, value: f32) {
    (&mut *oscillator).master_amplitude = value;
}

#[no_mangle]
unsafe extern "C" fn set_oscillator_default_frequency(oscillator: *mut Oscillator, value: f32) {
    (&mut *oscillator).default_frequency = value;
}

#[no_mangle]
unsafe extern "C" fn set_oscillator_settings(
    oscillator: *mut Oscillator,
    mode: WaveShapes,
    max_value: f32,
    min_value: f32,
    master_amplitude: f32,
    default_frequency: f32,
) {
    (&mut *oscillator).mode = mode;
    (&mut *oscillator).max_value = max_value;
    (&mut *oscillator).min_value = min_value;
    (&mut *oscillator).master_amplitude = master_amplitude;
    (&mut *oscillator).default_frequency = default_frequency;
}

#[no_mangle]
unsafe extern "C" fn process_oscillator(
    oscillator: *mut Oscillator,
    dst_buffer: *mut Buffer,
    amplitude_buffer: *mut Buffer,
    frequency_buffer: *mut Buffer,
    sample_rate: usize,
) {
    let oscillator = &mut *oscillator;
    let Some(dst_buffer) = (&*dst_buffer).interpret_as::<f32>() else {
        return;
    };

    let amplitude = match amplitude_buffer.is_null() {
        false => (&*amplitude_buffer).interpret_as::<f32>(),
        true => None,
    };

    let frequency = match frequency_buffer.is_null() {
        false => (&*frequency_buffer).interpret_as::<f32>(),
        true => None,
    };

    oscillator.process(dst_buffer, amplitude, frequency, sample_rate);
}
