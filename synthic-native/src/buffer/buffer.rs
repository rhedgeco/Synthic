use std::{
    alloc::{alloc_zeroed, dealloc, handle_alloc_error, Layout},
    marker::PhantomData,
    num::NonZeroUsize,
    ptr::NonNull,
};

use super::TypedBuffer;

impl Drop for Buffer {
    fn drop(&mut self) {
        if self.item_layout.size() > 0 && self.size() > 0 {
            unsafe { dealloc(self.data.as_ptr(), self.array_layout()) };
        }
    }
}

#[derive(Debug)]
pub struct Buffer {
    item_layout: Layout,
    data: NonNull<u8>,
    size: usize,
}

impl Buffer {
    pub fn new(item_layout: Layout, buffer_size: usize) -> Self {
        match NonZeroUsize::new(buffer_size) {
            None => Self {
                item_layout,
                data: unsafe { NonNull::new_unchecked(item_layout.align() as *mut u8) },
                size: 0,
            },
            Some(buffer_size) => {
                if item_layout.size() == 0 {
                    Self {
                        item_layout,
                        data: unsafe { NonNull::new_unchecked(item_layout.align() as *mut u8) },
                        size: buffer_size.get(),
                    }
                } else {
                    Self {
                        item_layout,
                        data: Self::alloc_buffer(item_layout, buffer_size),
                        size: buffer_size.get(),
                    }
                }
            }
        }
    }

    pub fn rebuild(&mut self, size: usize) {
        *self = Self::new(self.item_layout, size);
    }

    pub fn ptr(&self) -> *mut u8 {
        self.data.as_ptr()
    }

    pub fn size(&self) -> usize {
        self.size
    }

    pub fn item_layout(&self) -> Layout {
        self.item_layout
    }

    pub fn array_layout(&self) -> Layout {
        let array_size = self.item_layout.size() * self.size;
        unsafe { Layout::from_size_align_unchecked(array_size, self.item_layout.align()) }
    }

    pub fn interpret_as<T>(&self) -> Option<TypedBuffer<T>> {
        TypedBuffer::interpret(self)
    }

    pub fn get(&self, index: usize) -> Option<*mut u8> {
        if index >= self.size {
            return None;
        }

        if self.item_layout.size() == 0 {
            return Some(self.ptr());
        }

        let ptr_offset = index * self.item_layout.pad_to_align().size();
        Some(unsafe { self.data.as_ptr().add(ptr_offset) })
    }

    pub fn iter(&self) -> Iter {
        Iter {
            size: self.size,
            ptr_delta: self.item_layout().pad_to_align().size(),
            current_ptr: self.data.as_ptr(),
            current_index: 0,
            _lt: PhantomData,
        }
    }

    fn alloc_buffer(item_layout: Layout, buffer_size: NonZeroUsize) -> NonNull<u8> {
        let array_align = item_layout.align();
        let padded_item_size = item_layout.pad_to_align().size();
        let array_size = padded_item_size * buffer_size.get();
        let array_layout = unsafe { Layout::from_size_align_unchecked(array_size, array_align) };
        let data_ptr = match array_layout.size() {
            0 => array_layout.align() as *mut u8,
            _ => {
                let data_ptr = unsafe { alloc_zeroed(array_layout) };
                if data_ptr.is_null() {
                    handle_alloc_error(array_layout);
                }
                data_ptr
            }
        };
        unsafe { NonNull::new_unchecked(data_ptr) }
    }
}

pub struct Iter<'a> {
    size: usize,
    ptr_delta: usize,
    current_ptr: *mut u8,
    current_index: usize,
    _lt: PhantomData<&'a ()>,
}

impl<'a> Iterator for Iter<'a> {
    type Item = *mut u8;

    fn next(&mut self) -> Option<Self::Item> {
        if self.current_index >= self.size {
            return None;
        }

        let item_ptr = self.current_ptr;
        self.current_ptr = unsafe { self.current_ptr.add(self.ptr_delta) };
        self.current_index += 1;
        Some(item_ptr)
    }
}

impl<'a> IntoIterator for &'a Buffer {
    type Item = *mut u8;
    type IntoIter = Iter<'a>;

    fn into_iter(self) -> Self::IntoIter {
        self.iter()
    }
}

impl<'a> IntoIterator for &'a mut Buffer {
    type Item = *mut u8;
    type IntoIter = Iter<'a>;

    fn into_iter(self) -> Self::IntoIter {
        self.iter()
    }
}
