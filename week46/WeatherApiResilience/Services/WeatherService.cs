using WeatherApiResilience.Models;

namespace WeatherApiResilience.Services;

public class WeatherService(
    HttpClient httpClient, 
    IConfiguration configuration) : IWeatherService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _apiKey = configuration["WeatherApi:ApiKey"] ?? string.Empty;
    private readonly string _baseUrl = configuration["WeatherApi:BaseUrl"] ?? string.Empty;
    
    public async Task<WeatherResponse?> GetCurrentWeatherAsync(string city)
    {
        var url = $"{_baseUrl}/weather?q={city}&appid={_apiKey}&units=metric";
        var response = await _httpClient.GetAsync(url);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        
        return await response.Content.ReadFromJsonAsync<WeatherResponse>();
    }
}