using SlidingWindow.Logic.Interfaces;
using System;

namespace SlidingWindow.Logic.Services
{
    class MockTimestamp : ITimestamp
    {
        public long GetTimestamp()
        {
            throw new NotImplementedException();
        }
    }
}
