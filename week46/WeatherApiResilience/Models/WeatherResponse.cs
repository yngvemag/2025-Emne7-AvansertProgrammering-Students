using System.Text.Json.Serialization;

namespace WeatherApiResilience.Models;

public class WeatherResponse
{    
    [JsonPropertyName("id")]
    public required long Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }
    
    [JsonPropertyName("weather")]
    public required List<WeatherDescription> WeatherDescription { get; init; }

    [JsonPropertyName("main")]
    public required MainWeatherData Main { get; init; }

    [JsonPropertyName("base")]
    public required string Base { get; init; }

    [JsonPropertyName("visibility")]
    public required long Visibility { get; init; }

    [JsonPropertyName("timezone")]
    public required long Timezone { get; init; }
}

public class MainWeatherData
{
    [JsonPropertyName("temp")]
    public required double Temperature { get; init; }

    [JsonPropertyName("feels_like")]
    public required double FeelsLike { get; init; }

    [JsonPropertyName("temp_min")]
    public required double TempMin { get; init; }

    [JsonPropertyName("temp_max")]
    public required double TempMax { get; init; }

    [JsonPropertyName("pressure")]
    public required int Pressure { get; init; }

    [JsonPropertyName("humidity")]
    public required int Humidity { get; init; }
}

public class WeatherDescription
{
    [JsonPropertyName("id")]
    public required long Id { get; init; }

    [JsonPropertyName("main")]
    public required string Main { get; init; }

    [JsonPropertyName("description")]
    public required string Description { get; init; }

    [JsonPropertyName("icon")]
    public required string Icon { get; init; }
}