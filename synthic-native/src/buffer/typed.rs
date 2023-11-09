use std::{alloc::Layout, marker::PhantomData};

use crate::Buffer;

pub struct TypedBuffer<'a, T> {
    buffer: &'a Buffer,
    _type: PhantomData<&'a T>,
}

impl<'a, T> TypedBuffer<'a, T> {
    pub fn interpret(buffer: &'a Buffer) -> Option<Self> {
        if buffer.item_layout() != Layout::new::<T>() {
            return None;
        }

        Some(Self {
            buffer,
            _type: PhantomData,
        })
    }

    pub fn untyped(&self) -> &Buffer {
        self.buffer
    }

    pub fn size(&self) -> usize {
        self.buffer.size()
    }

    pub fn get(&self, index: usize) -> Option<&T> {
        let ptr = self.buffer.get(index)? as *mut T;
        Some(unsafe { &*ptr })
    }

    pub fn get_mut(&mut self, index: usize) -> Option<&mut T> {
        let ptr = self.buffer.get(index)? as *mut T;
        Some(unsafe { &mut *ptr })
    }

    pub fn iter(&self) -> Iter<T> {
        self.into_iter()
    }

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
