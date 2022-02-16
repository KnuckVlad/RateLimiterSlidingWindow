using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SlidingWindow.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiterWebApiExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IRateLimiter _rateLimiter;
        private readonly ILogger<WeatherForecastController> _logger;

        /// <summary>
        /// Example controller: using rate limiter in real web Api
        /// </summary>
        /// <param name="logger"></param>
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IRateLimiter rateLimiter)
        {
            _logger = logger;
            _rateLimiter = rateLimiter;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public IActionResult Get()
        {
            // Check rate limiter for request Limit
            if (_rateLimiter.IsLimited())
                return StatusCode(StatusCodes.Status429TooManyRequests);

            var rng = new Random();
            var forecastArr = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
            return Ok(forecastArr);
        }
    }
}
