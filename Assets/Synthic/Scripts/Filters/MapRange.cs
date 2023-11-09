using System.Collections.Generic;
using Synthic.Handlers.Extensions;
using UnityEngine;

namespace Synthic.Filters
{
    public class MapRange : BufferNode
    {
        [SerializeField] private InputPort<float> inputSource;
        [SerializeField] private float maxInput = 1;
        [SerializeField] private float minInput = -1;
        [SerializeField] private float maxOutput = 1;
        [SerializeField] private float minOutput;

        protected override IEnumerable<OutputPort> RegisterOutputs()
        {
            yield return OutputPort.Construct<float>("output");
        }

        protected override void ProcessCache(long sampleTime, BufferCache bufferCache)
        {
            var outCache = bufferCache.GetChannelBuffer<float>(0);
            if (!inputSource.GetSourceData(sampleTime, outCache)) outCache.SetAllValues(0);
            else outCache.MapValues(minInput, maxInput, minOutput, maxOutput);
        }
    }
}
