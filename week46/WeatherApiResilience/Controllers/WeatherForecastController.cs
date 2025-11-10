using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using WeatherApiResilience.Models;
using WeatherApiResilience.Services;

namespace WeatherApiResilience.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(
    IWeatherService weatherService,
    ILogger<WeatherForecastController> logger) : ControllerBase
{
    private readonly IWeatherService _weatherService = weatherService;
    private readonly ILogger<WeatherForecastController> _logger = logger;

    [HttpGet("{city}", Name = "GetWeatherForecast")]
    public async Task<ActionResult<WeatherResponse>> GetWeatherForecastAsync(string city)
    {
        var weather = await _weatherService.GetCurrentWeatherAsync(city);
        return weather is not null
            ? Ok(weather)
            : NotFound($"Weather data for city '{city}' not found.");
    }
}
