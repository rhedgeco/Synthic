use std::{alloc::Layout, marker::PhantomData};

use crate::Buffer;

/// A typed wrapper over the [`Buffer`] type.
///
/// This can only be constructed if the target buffer has the same `item_layout` as `T`
pub struct TypedBuffer<'a, T> {
    buffer: &'a Buffer,
    _type: PhantomData<&'a T>,
}

impl<'a, T> TypedBuffer<'a, T> {
    /// Interprets a [`Buffer`] as a new typed buffer.
    ///
    /// Returns `None` if `T` does not match the buffers `item_layout``
    pub fn interpret(buffer: &'a Buffer) -> Option<Self> {
        if buffer.item_layout() != Layout::new::<T>() {
            return None;
        }

        Some(Self {
            buffer,
            _type: PhantomData,
        })
    }

    /// Returns a reference to the underlying raw [`Buffer`]
    pub fn untyped(&self) -> &Buffer {
        self.buffer
    }

    /// Returns the amount of items the buffer can contain
    pub fn size(&self) -> usize {
        self.buffer.size()
    }

    /// Returns a reference to the item at `index`
    ///
    /// Returns `None` if the `index` is out of bounds
    pub fn get(&self, index: usize) -> Option<&T> {
        let ptr = self.buffer.get(index)? as *mut T;
        Some(unsafe { &*ptr })
    }

    /// Returns a mutable reference to the item at `index`
    ///
    /// Returns `None` if the `index` is out of bounds
    pub fn get_mut(&mut self, index: usize) -> Option<&mut T> {
        let ptr = self.buffer.get(index)? as *mut T;
        Some(unsafe { &mut *ptr })
    }

    /// Returns an iterator over all the items in the buffer
    pub fn iter(&self) -> Iter<T> {
        self.into_iter()
    }

    /// Returns a mutable iterator over all the items in the buffer
    pub fn iter_mut(&mut self) -> IterMut<T> {
        self.into_iter()
    }
}

pub struct Iter<'a, T> {
    inner: crate::buffer::Iter<'a>,
    _type: PhantomData<*const T>,
}

impl<'a, T: 'a> Iterator for Iter<'a, T> {
    type Item = &'a T;

    fn next(&mut self) -> Option<Self::Item> {
        let ptr = self.inner.next()? as *mut T;
        Some(unsafe { &*ptr })
    }
}

pub struct IterMut<'a, T> {
    inner: crate::buffer::Iter<'a>,
    _type: PhantomData<*const T>,
}

impl<'a, T: 'a> Iterator for IterMut<'a, T> {
    type Item = &'a mut T;

    fn next(&mut self) -> Option<Self::Item> {
        let ptr = self.inner.next()? as *mut T;
        Some(unsafe { &mut *ptr })
    }
}

impl<'a, T> IntoIterator for TypedBuffer<'a, T> {
    type Item = &'a mut T;
    type IntoIter = IterMut<'a, T>;
    fn into_iter(self) -> Self::IntoIter {
        IterMut {
            inner: self.buffer.iter(),
            _type: PhantomData,
        }
    }
}

impl<'a, 'r, T> IntoIterator for &'r TypedBuffer<'a, T> {
    type Item = &'a T;
    type IntoIter = Iter<'a, T>;
    fn into_iter(self) -> Self::IntoIter {
        Iter {
            inner: self.buffer.iter(),
            _type: PhantomData,
        }
    }
}

impl<'a, 'r, T> IntoIterator for &'r mut TypedBuffer<'a, T> {
    type Item = &'a mut T;
    type IntoIter = IterMut<'a, T>;
    fn into_iter(self) -> Self::IntoIter {
        IterMut {
            inner: self.buffer.iter(),
            _type: PhantomData,
        }
    }
}
