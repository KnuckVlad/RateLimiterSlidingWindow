using SlidingWindow.Logic.Interfaces;
using System;

namespace SlidingWindow.Logic
{
    public class SlidingWindow
    {
        // Object for locking Thread
        private readonly object _concurrencySyncObject = new object();

        private readonly ITimestamp _timestamp;
        private readonly long _requestIntervalTicks;
        private readonly int _requestLimit;

        private long? _windowStartTime;
        private int _previousRequestCount;
        private int _requestCount;

        public SlidingWindow(ITimestamp timestamp, int requestLimit, int requestIntervalMs)
        {
            _timestamp = timestamp;
            _requestLimit = requestLimit;
            _requestIntervalTicks = requestIntervalMs * TimeSpan.TicksPerMillisecond;
        }

        public bool IsPassingRequest()
        {
            var isPassed = false;
            lock (_concurrencySyncObject)
            {
                var currentTime = _timestamp.GetTimestamp();
                long elapsedTime = GetElapsedTime(currentTime);

                if (_windowStartTime.HasValue)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    _windowStartTime = currentTime;
                }
                double weightedRequestCount = EvaluateWeight(elapsedTime);
                if (weightedRequestCount <= _requestLimit)
                {
                    _requestCount++;
                    isPassed = true;
                }
            }
            return isPassed;
        }

        private double EvaluateWeight(long elapsedTime)
        {
            return _previousRequestCount * ((double)(_requestIntervalTicks - elapsedTime) / _requestIntervalTicks) + _requestCount + 1;
        }

        private long GetElapsedTime(long currentTime)
        {
            return _windowStartTime.HasValue ? (currentTime - _windowStartTime.Value) : 0;
        }
    }
}
