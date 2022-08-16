using System;
using System.Runtime.InteropServices;
using Synthic.Native.Midi;
using Unity.Mathematics;

namespace Synthic.Data
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NoteState
    {
        private Adsr _adsr;
        private float _velocity;

        public MidiNote.Signal Signal { get; private set; }
        public Adsr.State State { get; private set; }
        public float StateTime { get; private set; }
        public double Phase { get; private set; }
        public float NetVelocity { get; private set; }

        public void TurnOn(float initialPhase, float velocity, in Adsr adsr)
        {
            _adsr = adsr;
            _velocity = velocity;
            Signal = MidiNote.Signal.On;
            State = Adsr.State.Attack;
            StateTime = 0;
            Phase = math.max(initialPhase, 0) % 1;
        }

        public void Advance(float timeDelta, double phaseDelta)
        {
            timeDelta = math.max(timeDelta, 0);
            phaseDelta = math.max(phaseDelta, 0);
            StateTime += timeDelta;
            Phase = (Phase + phaseDelta) % 1;

            if (State == Adsr.State.Attack && StateTime > _adsr.AttackTime)
            {
                StateTime -= _adsr.AttackTime;
                State = Adsr.State.Decay;
            }

            if (State == Adsr.State.Decay && StateTime > _adsr.DecayTime)
            {
                StateTime -= _adsr.DecayTime;
                State = Adsr.State.Sustain;
            }

            if (Signal == MidiNote.Signal.Off && State == Adsr.State.Sustain)
            {
                StateTime = 0;
                State = Adsr.State.Release;
            }

            NetVelocity = State switch
            {
                Adsr.State.Attack => StateTime / _adsr.AttackTime * _velocity,
                Adsr.State.Decay => math.lerp(1, _adsr.SustainLevel, StateTime / _adsr.DecayTime) * _velocity,
                Adsr.State.Sustain => _adsr.SustainLevel * _velocity,
                Adsr.State.Release => 
                    math.max(math.lerp(_adsr.SustainLevel, 0, StateTime / _adsr.ReleaseTime), 0) * _velocity,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void TurnOff()
        {
            Signal = MidiNote.Signal.Off;
        }
    }
}