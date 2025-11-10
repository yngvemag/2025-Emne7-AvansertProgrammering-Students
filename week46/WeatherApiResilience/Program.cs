using Polly;
using Polly.Retry;
using Scalar.AspNetCore;
using WeatherApiResilience.Services;

var builder = WebApplication.CreateBuilder(args);


// Use standard resilience handler
//builder.Services.AddHttpClient<IWeatherService, WeatherService>()
//    .AddStandardResilienceHandler();


// Use custom resilience handler
builder.Services.AddHttpClient<IWeatherService, WeatherService>()
     .AddResilienceHandler(
         "default", 
         static _ => {});

builder.Services.AddResiliencePipeline("default", o =>
{
    o.AddRetry(new RetryStrategyOptions()
    {
        ShouldHandle = new PredicateBuilder().Handle<Exception>(),
        Delay = TimeSpan.FromSeconds(2),
        MaxRetryAttempts = 3,
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true
    });
});


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
