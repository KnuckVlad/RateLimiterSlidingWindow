using Microsoft.Extensions.Options;
using SlidingWindow.Logic;
using SlidingWindow.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiterWebApiExample
{
    /// <summary>
    /// Option class for DI container
    /// </summary>
    public class RateLimiterOptions
    {
       public int RequestLimit { get; set; }
       public int RequestIntervalMs { get; set; }
    }

    /// <summary>
    /// Inheritance for DI container (Used in .NET Core)
    /// </summary>
    public class SlidingWindowRateLimiterDI : SlidingWindowRateLimiter
    {
        public SlidingWindowRateLimiterDI(ITimestamp timestamp, IOptions<RateLimiterOptions> options) : 
            base(timestamp, options.Value.RequestLimit, options.Value.RequestIntervalMs)
        {
        }
    }
}
