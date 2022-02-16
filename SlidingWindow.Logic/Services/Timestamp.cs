using SlidingWindow.Logic.Interfaces;
using System.Diagnostics;

namespace SlidingWindow.Logic.Services
{
    public class Timestamp : ITimestamp
    {
        public long GetTimestamp()
        {
            return Stopwatch.GetTimestamp();
        }
    }
}
