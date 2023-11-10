use crate::{oscillator::OscillatorSettings, Buffer, SimpleOscillator};

use super::WaveShape;

#[no_mangle]
unsafe extern "C" fn create_oscillator_settings(
    shape: WaveShape,
    voices: usize,
    voice_pan: f32,
    voice_frequency: f32,
    voice_amplitude: f32,
) -> *mut OscillatorSettings {
    Box::into_raw(Box::new(OscillatorSettings::new(
        shape,
        voices,
        voice_pan,
        voice_frequency,
        voice_amplitude,
    )))
}

#[no_mangle]
unsafe extern "C" fn dispose_oscillator_settings(settings: *mut OscillatorSettings) {
    drop(Box::from_raw(settings))
}

#[no_mangle]
unsafe extern "C" fn create_simple_oscillator(
    copy_settings: *mut OscillatorSettings,
    master_amplitude: f32,
    default_frequency: f32,
) -> *mut SimpleOscillator {
    Box::into_raw(Box::new(SimpleOscillator::new(
        (*copy_settings).clone(),
        master_amplitude,
        default_frequency,
    )))
}

#[no_mangle]
unsafe extern "C" fn dispose_simple_oscillator(core: *mut SimpleOscillator) {
    drop(Box::from_raw(core))
}

#[no_mangle]
unsafe extern "C" fn set_oscillator_settings_shape(
    settings: *mut OscillatorSettings,
    value: WaveShape,
) {
    (*settings).shape = value;
}

#[no_mangle]
unsafe extern "C" fn set_oscillator_settings_voices(
    settings: *mut OscillatorSettings,
    value: usize,
) {
    (*settings).set_voices(value);
}

#[no_mangle]
unsafe extern "C" fn set_oscillator_settings_voice_pan(
    settings: *mut OscillatorSettings,
    value: f32,
) {
    (*settings).voice_pan = value;
}

#[no_mangle]
unsafe extern "C" fn set_oscillator_settings_voice_frequency(
    settings: *mut OscillatorSettings,
    value: f32,
) {
    (*settings).voice_frequency = value;
}

#[no_mangle]
unsafe extern "C" fn set_oscillator_settings_voice_amplitude(
    settings: *mut OscillatorSettings,
    value: f32,
) {
    (*settings).voice_amplitude = value;
}

#[no_mangle]
unsafe extern "C" fn set_simple_oscillator_master_amplitude(
    oscillator: *mut SimpleOscillator,
    value: f32,
) {
    (*oscillator).master_amplitude = value;
}

#[no_mangle]
unsafe extern "C" fn set_simple_oscillator_default_frequency(
    oscillator: *mut SimpleOscillator,
    value: f32,
) {
    (*oscillator).default_frequency = value;
}

#[no_mangle]
unsafe extern "C" fn set_simple_oscillator_settings(
    oscillator: *mut SimpleOscillator,
    copy_settings: *mut OscillatorSettings,
) {
    (*oscillator).settings = (*copy_settings).clone();
}

#[no_mangle]
unsafe extern "C" fn process_simple_oscillator(
    oscillator: *mut SimpleOscillator,
    left_buffer: *mut Buffer,
    right_buffer: *mut Buffer,
    amplitude: *mut Buffer,
    frequency: *mut Buffer,
    sample_rate: f64,
) {
    let Some(left_buffer) = (*left_buffer).interpret_as::<f32>() else {
        return;
    };
    let Some(right_buffer) = (*right_buffer).interpret_as::<f32>() else {
        return;
    };
    let amplitude = match amplitude.is_null() {
        false => (*amplitude).interpret_as::<f32>(),
        true => None,
    };
    let frequency = match frequency.is_null() {
        false => (*frequency).interpret_as::<f32>(),
        true => None,
    };

    (*oscillator).process_buffer(left_buffer, right_buffer, amplitude, frequency, sample_rate);
}

// #[no_mangle]
// unsafe extern "C" fn create_simple_oscillator(
//     shape: WaveShape,
//     max_value: f32,
//     min_value: f32,
//     default_amplitude: f32,
//     default_frequency: f32,
// ) -> *mut SimpleOscillator {
//     Box::into_raw(Box::new(SimpleOscillator::new())))
// }

// #[no_mangle]
// unsafe extern "C" fn dispose_oscillator(oscillator: *mut Oscillator) {
//     drop(Box::from_raw(oscillator))
// }

// #[no_mangle]
// unsafe extern "C" fn set_oscillator_mode(oscillator: *mut Oscillator, mode: WaveShape) {
//     (&mut *oscillator).mode = mode;
// }

// #[no_mangle]
// unsafe extern "C" fn set_oscillator_max_value(oscillator: *mut Oscillator, value: f32) {
//     (&mut *oscillator).max_value = value;
// }

// #[no_mangle]
// unsafe extern "C" fn set_oscillator_min_value(oscillator: *mut Oscillator, value: f32) {
//     (&mut *oscillator).min_value = value;
// }

// #[no_mangle]
// unsafe extern "C" fn set_oscillator_master_amplitude(oscillator: *mut Oscillator, value: f32) {
//     (&mut *oscillator).master_amplitude = value;
// }

// #[no_mangle]
// unsafe extern "C" fn set_oscillator_default_frequency(oscillator: *mut Oscillator, value: f32) {
//     (&mut *oscillator).default_frequency = value;
// }

// #[no_mangle]
// unsafe extern "C" fn set_oscillator_settings(
//     oscillator: *mut Oscillator,
//     mode: WaveShape,
//     max_value: f32,
//     min_value: f32,
//     master_amplitude: f32,
//     default_frequency: f32,
// ) {
//     (&mut *oscillator).mode = mode;
//     (&mut *oscillator).max_value = max_value;
//     (&mut *oscillator).min_value = min_value;
//     (&mut *oscillator).master_amplitude = master_amplitude;
//     (&mut *oscillator).default_frequency = default_frequency;
// }

// #[no_mangle]
// unsafe extern "C" fn process_oscillator(
//     oscillator: *mut Oscillator,
//     dst_buffer: *mut Buffer,
//     amplitude_buffer: *mut Buffer,
//     frequency_buffer: *mut Buffer,
//     sample_rate: usize,
// ) {
//     let oscillator = &mut *oscillator;
//     let Some(dst_buffer) = (&*dst_buffer).interpret_as::<f32>() else {
//         return;
//     };

//     let amplitude = match amplitude_buffer.is_null() {
//         false => (&*amplitude_buffer).interpret_as::<f32>(),
//         true => None,
//     };

//     let frequency = match frequency_buffer.is_null() {
//         false => (&*frequency_buffer).interpret_as::<f32>(),
//         true => None,
//     };

//     oscillator.process(dst_buffer, amplitude, frequency, sample_rate);
// }
