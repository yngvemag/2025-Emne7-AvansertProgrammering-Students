using WeatherApiResilience.Models;

namespace WeatherApiResilience.Services;

public interface IWeatherService
{
    Task<WeatherResponse?> GetCurrentWeatherAsync(string city);
}