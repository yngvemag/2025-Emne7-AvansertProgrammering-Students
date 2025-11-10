using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace RateLimitingBasics.Controllers;

[ApiController]
[Route("[controller]")]
public class RateLimitTestsController : ControllerBase
{
    private static object Payload(string policy)
        => new
        {
            policy,
            utc = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
            ticks = DateTime.UtcNow.Ticks
        };

    [EnableRateLimiting("fixed")]
    [HttpGet("fixed")]
    public IActionResult FixedRate() => Ok(Payload("fixed"));
    
    [EnableRateLimiting("sliding")]
    [HttpGet("sliding")]
    public IActionResult Sliding() => Ok(Payload("sliding"));
    
    [EnableRateLimiting("token_bucket")]
    [HttpGet("token-bucket")]
    public IActionResult TokenBucket() => Ok(Payload("token_bucket"));
    
}