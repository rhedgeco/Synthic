use crate::{Buffer, Notes};

use super::{synth::OscSettings, SynthOscillator, WaveShapes};

#[no_mangle]
unsafe extern "C" fn create_synth_oscillator(
    voices: usize,
    wave: WaveShapes,
    voice_pan: f32,
    voice_frequency: f32,
    voice_amplitude: f32,
    master_amplitude: f32,
) -> *mut SynthOscillator {
    Box::into_raw(Box::new(SynthOscillator::new(
        OscSettings::new(voices, wave, voice_pan, voice_frequency, voice_amplitude),
        master_amplitude,
    )))
}

#[no_mangle]
unsafe extern "C" fn dispose_synth_oscillator(oscillator: *mut SynthOscillator) {
    drop(Box::from_raw(oscillator))
}

#[no_mangle]
unsafe extern "C" fn set_synth_oscillator_wave(
    oscillator: *mut SynthOscillator,
    value: WaveShapes,
) {
    (&mut *oscillator).settings.wave = value;
}

#[no_mangle]
unsafe extern "C" fn set_synth_oscillator_master_amplitude(
    oscillator: *mut SynthOscillator,
    value: f32,
) {
    (&mut *oscillator).master_amplitude = value;
}

#[no_mangle]
unsafe extern "C" fn set_synth_oscillator_voices(oscillator: *mut SynthOscillator, value: usize) {
    (&mut *oscillator).settings.set_voices(value);
}

#[no_mangle]
unsafe extern "C" fn set_synth_oscillator_voice_pan(oscillator: *mut SynthOscillator, value: f32) {
    (&mut *oscillator).settings.voice_pan = value;
}

#[no_mangle]
unsafe extern "C" fn set_synth_oscillator_voice_frequency(
    oscillator: *mut SynthOscillator,
    value: f32,
) {
    (&mut *oscillator).settings.voice_frequency = value;
}

#[no_mangle]
unsafe extern "C" fn set_synth_oscillator_voice_amplitude(
    oscillator: *mut SynthOscillator,
    value: f32,
) {
    (&mut *oscillator).settings.voice_amplitude = value;
}

#[no_mangle]
unsafe extern "C" fn set_synth_oscillator_settings(
    oscillator: *mut SynthOscillator,
    voices: usize,
    wave: WaveShapes,
    voice_pan: f32,
    voice_frequency: f32,
    voice_amplitude: f32,
    master_amplitude: f32,
) {
    (&mut *oscillator).settings.set_voices(voices);
    (&mut *oscillator).settings.wave = wave;
    (&mut *oscillator).settings.voice_pan = voice_pan;
    (&mut *oscillator).settings.voice_amplitude = voice_amplitude;
    (&mut *oscillator).settings.voice_frequency = voice_frequency;
    (&mut *oscillator).master_amplitude = master_amplitude;
}

#[no_mangle]
unsafe extern "C" fn process_synth_oscillator(
    oscillator: *mut SynthOscillator,
    left_buffer: *mut Buffer,
    right_buffer: *mut Buffer,
    notes_buffer: *mut Buffer,
    sample_rate: usize,
) {
    let oscillator = &mut *oscillator;
    let Some(left_buffer) = (&*left_buffer).interpret_as::<f32>() else {
        return;
    };

    let Some(right_buffer) = (&*right_buffer).interpret_as::<f32>() else {
        return;
    };

    let Some(notes_buffer) = (&*notes_buffer).interpret_as::<Notes>() else {
        return;
    };

    oscillator.process(left_buffer, right_buffer, notes_buffer, sample_rate);
}
