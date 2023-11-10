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

/// Untyped raw data storage used for working with data across FFI boundaries.
///
/// Buffers are constructed with manual layouts and will never know any information about
/// what is being stored inside. This is unfortunate as it will never be able to call the drop
/// function on the "contained" items, but it allows it to fit into a niche of storing data
/// that is defined in other languages and passed via FFI.
#[derive(Debug)]
pub struct Buffer {
    item_layout: Layout,
    data: NonNull<u8>,
    size: usize,
}

impl Buffer {
    /// Returns a new buffer with a specific `item_layout` and `buffer_size`
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

    /// Re-allocates the buffer in place with a new size.
    ///
    /// Erases the old data that was contained in the first place.
    pub fn rebuild(&mut self, size: usize) {
        *self = Self::new(self.item_layout, size);
    }

    /// Get a raw ptr to the start of the buffer
    pub fn ptr(&self) -> *mut u8 {
        self.data.as_ptr()
    }

    /// Returns the amount of items the buffer can contain
    pub fn size(&self) -> usize {
        self.size
    }

    /// Returns the layout of the contained items
    pub fn item_layout(&self) -> Layout {
        self.item_layout
    }

    /// Returns the layout of the entire buffer
    pub fn array_layout(&self) -> Layout {
        let array_size = self.item_layout.size() * self.size;
        unsafe { Layout::from_size_align_unchecked(array_size, self.item_layout.align()) }
    }

    /// Reinterprets the buffer into a [`TypedBuffer`] as long as the layout matches.
    ///
    /// Returns `None` if the layout of `T` does not match the `item_layout`
    pub fn interpret_as<T>(&self) -> Option<TypedBuffer<T>> {
        TypedBuffer::interpret(self)
    }

    /// Gets the ptr at the specified buffer `index`
    ///
    /// Returns `None` if the index is out of bounds
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

    /// Returns an itertor over every pointer in the buffer
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

/// Iterates over every ptr in a [`Buffer`]
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
