using System;
using Synthic.Handlers;
using UnityEngine;

namespace Synthic
{
    [Serializable]
    public struct InputPort<T> where T : unmanaged
    {
        [SerializeField] private BufferNode sourceNode;
        [SerializeField] private int sourceChannel;

        public bool GetSourceData(long sampleTime, Buffer<T> buffer)
        {
            if (sourceNode == null) return false;
            sourceNode.GetOutputData(sampleTime, sourceChannel, buffer);
            return true;
        }

        internal Buffer<T> GetSourceBuffer(long sampleTime)
        {
            return sourceNode == null ? null : sourceNode.GetOutputBuffer<T>(sampleTime, sourceChannel);
        }
    }
}
