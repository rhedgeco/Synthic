pub struct Note {
    amplitude: f32,
    frequency: f32,
}

impl Note {
    pub fn amplitude(&self) -> f32 {
        self.amplitude
    }

    pub fn frequency(&self) -> f32 {
        self.frequency
    }
}

pub struct NoteMut<'a> {
    amplitude: &'a mut f32,
    frequency: &'a mut f32,
}

impl<'a> NoteMut<'a> {
    pub fn amplitude(&self) -> f32 {
        *self.amplitude
    }

    pub fn frequency(&self) -> f32 {
        *self.frequency
    }

    pub fn amplitude_mut(&mut self) -> &mut f32 {
        self.amplitude
    }

    pub fn frequency_mut(&mut self) -> &mut f32 {
        self.frequency
    }
}

#[repr(C)]
pub struct Notes {
    amplitude: [f32; Self::NOTE_COUNT],
    frequency: [f32; Self::NOTE_COUNT],
}

impl Notes {
    pub const NOTE_COUNT: usize = 128;

    pub fn get_note(&self, index: usize) -> Note {
        Note {
            amplitude: self.amplitude[index],
            frequency: self.frequency[index],
        }
    }

    pub fn iter(&self) -> Iter {
        Iter {
            ampl_iter: self.amplitude.iter(),
            freq_iter: self.frequency.iter(),
        }
    }

    pub fn iter_mut(&mut self) -> IterMut {
        IterMut {
            ampl_iter: self.amplitude.iter_mut(),
            freq_iter: self.frequency.iter_mut(),
        }
    }
}

pub struct Iter<'a> {
    ampl_iter: std::slice::Iter<'a, f32>,
    freq_iter: std::slice::Iter<'a, f32>,
}

impl<'a> Iterator for Iter<'a> {
    type Item = Note;

    fn next(&mut self) -> Option<Self::Item> {
        let amplitude = *self.ampl_iter.next()?;
        let frequency = *self.freq_iter.next()?;
        Some(Note {
            amplitude,
            frequency,
        })
    }
}

pub struct IterMut<'a> {
    ampl_iter: std::slice::IterMut<'a, f32>,
    freq_iter: std::slice::IterMut<'a, f32>,
}

impl<'a> Iterator for IterMut<'a> {
    type Item = NoteMut<'a>;

    fn next(&mut self) -> Option<Self::Item> {
        let amplitude = self.ampl_iter.next()?;
        let frequency = self.freq_iter.next()?;
        Some(NoteMut {
            amplitude,
            frequency,
        })
    }
}

impl<'a> IntoIterator for &'a Notes {
    type Item = Note;
    type IntoIter = Iter<'a>;
    fn into_iter(self) -> Self::IntoIter {
        self.iter()
    }
}

impl<'a> IntoIterator for &'a mut Notes {
    type Item = NoteMut<'a>;
    type IntoIter = IterMut<'a>;
    fn into_iter(self) -> Self::IntoIter {
        self.iter_mut()
    }
}
