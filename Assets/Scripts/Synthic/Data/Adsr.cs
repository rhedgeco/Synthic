using Synthic.Native.Midi;
using Unity.Mathematics;
using UnityEngine;

namespace Synthic.Data
{
    [System.Serializable]
    public struct Adsr
    {
        [Min(0)] public float attack;
        [Min(0)] public float decay;
        [Range(0, 1)] public float sustain;
        [Min(0)] public float release;

        public Adsr(float attack = 0.1f, float decay = 0.1f, float sustain = 1, float release = 0.1f)
        {
            this.attack = math.max(attack, 0);
            this.decay = math.max(decay, 0);
            this.sustain = math.clamp(sustain, 0, 1);
            this.release = math.max(release, 0);
        }

        public float ProcessEnvelope(in NoteState state)
        {
            if (state.Time < attack) return (float) (state.Time / attack) * state.Velocity;
            if (state.Time < attack + decay)
                return (float) math.lerp(1, sustain, (state.Time - attack) / decay) * state.Velocity;
            if (state.Signal == MidiNote.Signal.On) return sustain * state.Velocity;
            double releaseTime = state.ReleaseTime < attack + decay ? attack + decay : state.ReleaseTime;
            releaseTime = state.Time - releaseTime;
            if (releaseTime < release) return (1 - (float) (releaseTime / release)) * state.Velocity;
            return 0;
        }
    }
}