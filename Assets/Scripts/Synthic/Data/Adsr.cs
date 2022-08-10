using Synthic.Native.Midi;
using Unity.Mathematics;
using UnityEngine;

namespace Synthic.Data
{
    [System.Serializable]
    public struct Adsr
    {
        public const float PowerMax = 2;
        public const float PowerMin = 1 / PowerMax;

        [SerializeField, Min(0)] private float attack;
        [SerializeField, Range(PowerMin, PowerMax)] private float attackPower;
        [SerializeField, Min(0)] private float decay;
        [SerializeField, Range(PowerMin, PowerMax)] private float decayPower;
        [SerializeField, Range(0, 1)] private float sustain;
        [SerializeField, Min(0)] private float release;
        [SerializeField, Range(PowerMin, PowerMax)] private float releasePower;

        public Adsr(
            float attack = 0.1f, float attackPower = 1f,
            float decay = 0.1f, float decayPower = 1f,
            float sustain = 1,
            float release = 0.1f, float releasePower = 1f)
        {
            this.attack = math.max(attack, 0);
            this.attackPower = math.clamp(attackPower, PowerMin, PowerMax);
            this.decay = math.max(decay, 0);
            this.decayPower = math.clamp(decayPower, PowerMin, PowerMax);
            this.sustain = math.clamp(sustain, 0, 1);
            this.release = math.max(release, 0);
            this.releasePower = math.clamp(releasePower, PowerMin, PowerMax);
        }

        public float ProcessEnvelope(in NoteState state)
        {
            double time = state.Time;
            float velocity = state.Velocity;

            // calculate attack phase
            if (time < attack) return (float) math.pow(time / attack, attackPower) * velocity;

            // calculate decay phase
            if (time - attack < decay)
                return (float) math.lerp(1, sustain, math.pow((time - attack) / decay, decayPower)) * velocity;

            // persist in sustain phase while note is on
            if (state.Signal == MidiNote.Signal.On) return sustain * velocity;

            // calculate release time
            double releaseTime = state.ReleaseTime;
            if (releaseTime < attack + decay) releaseTime = attack + decay;
            releaseTime = time - releaseTime;

            // calculate release phase
            if (releaseTime < release)
                return (float) (math.lerp(sustain, 0, math.pow(releaseTime / release, releasePower)) * velocity);

            // final end phase returns 0
            return 0;
        }
    }
}