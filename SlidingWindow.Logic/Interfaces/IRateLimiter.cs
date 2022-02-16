namespace SlidingWindow.Logic.Interfaces
{
    public interface IRateLimiter
    {
        /// <summary>
        /// indicates is request limited
        /// </summary>
        bool IsLimited();
    }
}
