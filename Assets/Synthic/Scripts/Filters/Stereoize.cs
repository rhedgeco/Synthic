using System.Collections.Generic;
using UnityEngine;

namespace Synthic.Filters
{
    public class Stereoize : BufferNode
    {
        [SerializeField] private InputPort<float> input;

        protected override IEnumerable<OutputPort> RegisterOutputs()
        {
            yield return OutputPort.Construct<float>("left");
            yield return OutputPort.Construct<float>("right");
        }

        protected override void ProcessCache(long sampleTime, BufferCache bufferCache)
        {
            var leftCache = bufferCache.GetChannelBuffer<float>(0);
            var rightCache = bufferCache.GetChannelBuffer<float>(1);
            if (!input.GetSourceData(sampleTime, leftCache)) leftCache.SetAllValues(0);
            leftCache.CopyToBuffer(rightCache);
        }
    }
}
