using Unity.Mathematics;
using UnityEngine;

namespace Synthic.Data
{
    [System.Serializable]
    public struct Adsr
    {
        public const float PowerMax = 2;
        public const float PowerMin = 1 / PowerMax;

        [SerializeField, Min(0)] private float attackTime;
        [SerializeField, Min(0)] private float decayTime;
        [SerializeField, Range(0, 1)] private float sustainLevel;
        [SerializeField, Min(0)] private float releaseTime;

        public float AttackTime
        {
            get => attackTime;
            set => attackTime = math.max(value, 0);
        }

        public float DecayTime
        {
            get => decayTime;
            set => decayTime = math.max(value, 0);
        }

        public float SustainLevel
        {
            get => sustainLevel;
            set => sustainLevel = math.clamp(value, 0, 1);
        }

        public float ReleaseTime
        {
            get => releaseTime;
            set => releaseTime = math.max(value, 0);
        }

        public enum State
        {
            Attack,
            Decay,
            Sustain,
            Release
        }
    }
}