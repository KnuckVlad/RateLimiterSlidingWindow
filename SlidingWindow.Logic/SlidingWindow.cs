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

        public bool IsLimited()
        {
            var isPassed = false;
            lock (_concurrencySyncObject)
            {
                long currentTime = _timestamp.GetTimestamp();
                long elapsedTime = GetElapsedTime(currentTime);

                if (_windowStartTime.HasValue)
                {
                    if (elapsedTime > _requestIntervalTicks * 2)
                        SetStartingWindowPeriod(ref elapsedTime, currentTime);
                    if (elapsedTime >= _requestIntervalTicks)
                        SetNextWindowPeriod();
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

        /// <summary>
        /// Set doubled window time
        /// This situation is similar to "starting point" after first request.
        /// </summary>
        /// <param name="elapsedTime">elapsed time parameter, ref is used to return value type.</param>
        /// <param name="currentTime">current timestamp in Ticks.</param>
        private void SetStartingWindowPeriod(ref long elapsedTime, long currentTime)
        {
            _windowStartTime = currentTime;
            _previousRequestCount = 0;
            _requestCount = 0;
            elapsedTime = 0;
        }

        /// <summary>
        /// request exceeded current window, scoring the result of the current window and moving to another.
        /// </summary>
        private void SetNextWindowPeriod()
        {
            // moving to next window, adding interval
            _windowStartTime += _requestIntervalTicks;
            // fixing previous request count 
            _previousRequestCount = _requestCount;
            // set counter to zero
            _requestCount = 0;
        }

        /// <summary>
        /// Weighted limit formula (using current and previous window statistics).
        /// </summary>
        /// <param name="elapsedTime">elapsed time parameter.</param>
        /// <returns></returns>
        private double EvaluateWeight(long elapsedTime)
        {
            return _previousRequestCount * ((double)(_requestIntervalTicks - elapsedTime) / _requestIntervalTicks) + _requestCount + 1;
        }

        /// <summary>
        /// gets delta between window start time and elapsed time.
        /// </summary>
        /// <param name="currentTime">current timestamp in Ticks.</param>
        /// <returns></returns>
        private long GetElapsedTime(long currentTime)
        {
            return _windowStartTime.HasValue ? (currentTime - _windowStartTime.Value) : 0;
        }
    }
}
