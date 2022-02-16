using Moq;
using SlidingWindow.Logic;
using SlidingWindow.Logic.Interfaces;
using System;
using Xunit;

namespace TestProject1
{
    public class RateLimiterTests
    {
        private readonly Mock<ITimestamp> _mockTimestamp = new Mock<ITimestamp>();

        private long Saturate(SlidingWindowRateLimiter rateLimiter, int requestLimit, long incrementTicks, long elapsedTicks)
        {
            for (int i = requestLimit - 1; i >= 0; i--)
            {
                _mockTimestamp.Setup(x => x.GetTimestamp()).Returns(elapsedTicks);
                // asserting that request are still under limit
                Assert.False(rateLimiter.IsLimited());

                elapsedTicks += incrementTicks;
            }

            // asserting that requests are overlimited
            Assert.True(rateLimiter.IsLimited());

            return elapsedTicks;
        }

        private long GetTicks(int msTime) => msTime * TimeSpan.TicksPerMillisecond;
        private long GetIncrementTicks(int requestLimit, int requestIntervalMs) => GetTicks(requestIntervalMs / requestLimit);



        [Theory]
        [InlineData(10, 1000)]
        public void RequestPassed_InitialWindowNotExists_Limiter(int requestLimit, int requestIntervalMs)
        {
            var slidingWindow = new SlidingWindowRateLimiter(_mockTimestamp.Object, requestLimit, requestIntervalMs);

            Saturate(slidingWindow, requestLimit, 0, 0);
        }

        [Theory]
        [InlineData(5, 5000)]
        public void RequestPassed_InitialWindowExists_Limiter(int requestLimit, int requestIntervalMs)
        {
            var slidingWindow = new SlidingWindowRateLimiter(_mockTimestamp.Object, requestLimit, requestIntervalMs);

            var toIncrementTicks = GetIncrementTicks(requestLimit, requestIntervalMs);
            var elapsedTicks = Saturate(slidingWindow, requestLimit, toIncrementTicks, 0);

            _mockTimestamp.Setup(x => x.GetTimestamp()).Returns(elapsedTicks);

            Assert.True(slidingWindow.IsLimited());
        }
    }
}
