using JwtPractice.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtPractice.Controllers
{
    [Authorize] //Bu attribute ile bu classtaki tüm endpointlere token zorunluluğu getiriyoruz.
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        
        private readonly IJwtAuthenticationService jwtAuthenticationService;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IServiceProvider service)
        {
            _logger = logger;
            jwtAuthenticationService = service.GetService<IJwtAuthenticationService>(); //IoC'den bir nesne türetmesini söylüyorum.
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet("getall")]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [AllowAnonymous]  //Bu attribute ile tokensiz istekte bulunabiliyoruz.
        [HttpPost("login")]
        public IActionResult Login(string userName, string password)
        {
            var result = jwtAuthenticationService.Authentication(userName, password);
            if (!String.IsNullOrEmpty(result))
            {
                return Ok(result);
            }
            return BadRequest();
        }
    }
}
