use std::{alloc::Layout, ptr::null_mut};

use crate::Buffer;

#[no_mangle]
unsafe extern "C" fn create_buffer(
    item_size: usize,
    item_align: usize,
    buffer_size: usize,
) -> *mut Buffer {
    let Ok(item_layout) = Layout::from_size_align(item_size, item_align) else {
        return null_mut();
    };

    Box::into_raw(Box::new(Buffer::new(item_layout, buffer_size)))
}

#[no_mangle]
unsafe extern "C" fn dispose_buffer(buffer: *mut Buffer) {
    drop(Box::from_raw(buffer))
}

#[no_mangle]
unsafe extern "C" fn rebuild_buffer(buffer: *mut Buffer, buffer_size: usize) {
    let buffer = &mut *buffer;
    buffer.rebuild(buffer_size);
}

#[no_mangle]
unsafe extern "C" fn buffer_ptr(buffer: *mut Buffer) -> *mut u8 {
    (&*buffer).ptr()
}

#[no_mangle]
unsafe extern "C" fn copy_to_buffer(src_buffer: *mut Buffer, dst_buffer: *mut Buffer) {
    let src_buffer = &mut *src_buffer;
    let dst_buffer = &mut *dst_buffer;
    if src_buffer.item_layout() != dst_buffer.item_layout() {
        return;
    }

    let min_size = src_buffer.size().min(dst_buffer.size());
    if min_size == 0 {
        return;
    }

    let src_ptr = src_buffer.ptr();
    let dst_ptr = dst_buffer.ptr();
    let copy_size = src_buffer.item_layout().pad_to_align().size() * min_size;
    std::ptr::copy_nonoverlapping(src_ptr, dst_ptr, copy_size);
}

#[no_mangle]
unsafe extern "C" fn copy_to_ptr(src_buffer: *mut Buffer, dst_ptr: *mut u8, dst_size: usize) {
    let src_buffer = &mut *src_buffer;
    let min_size = src_buffer.size().min(dst_size);
    if min_size == 0 {
        return;
    }

    let copy_size = src_buffer.item_layout().pad_to_align().size() * min_size;
    std::ptr::copy_nonoverlapping(src_buffer.ptr(), dst_ptr, copy_size);
}

#[no_mangle]
unsafe extern "C" fn copy_from_ptr(dst_buffer: *mut Buffer, src_ptr: *mut u8, src_size: usize) {
    let dst_buffer = &mut *dst_buffer;
    let min_size = dst_buffer.size().min(src_size);
    let copy_size = dst_buffer.item_layout().pad_to_align().size() * min_size;
    std::ptr::copy_nonoverlapping(src_ptr, dst_buffer.ptr(), copy_size);
}

#[no_mangle]
unsafe extern "C" fn set_buffer_values(buffer: *mut Buffer, src_ptr: *mut u8) {
    let buffer = &*buffer;
    let item_size = buffer.item_layout().pad_to_align().size();
    for sample in buffer {
        std::ptr::copy_nonoverlapping(src_ptr, sample, item_size);
    }
}

#[no_mangle]
unsafe extern "C" fn copy_to_ptr_channel(
    src_buffer: *mut Buffer,
    dst_ptr: *mut u8,
    dst_size: usize,
    total_channels: usize,
    target_channel: usize,
) -> bool {
    if target_channel >= total_channels {
        return false;
    }

    let src_buffer = &mut *src_buffer;
    let dst_channel_size = dst_size / total_channels;
    let item_count = src_buffer.size().min(dst_channel_size);
    let data_size = src_buffer.item_layout().pad_to_align().size();
    let dst_data_delta = data_size * total_channels;
    let mut dst_data_ptr = dst_ptr.add(target_channel * data_size);
    for src_data_ptr in src_buffer.iter().take(item_count) {
        std::ptr::copy_nonoverlapping(src_data_ptr, dst_data_ptr, data_size);
        dst_data_ptr = dst_data_ptr.add(dst_data_delta);
    }
    true
}

#[no_mangle]
unsafe extern "C" fn map_buffer_floats(
    buffer: *mut Buffer,
    in_min: f32,
    in_max: f32,
    out_min: f32,
    out_max: f32,
) {
    let Some(buffer) = (&*buffer).interpret_as::<f32>() else {
        return;
    };

    let slope = (out_max - out_min) / (in_max - in_min);
    for sample in buffer {
        *sample = (*sample - in_min) * slope + out_min;
    }
}
