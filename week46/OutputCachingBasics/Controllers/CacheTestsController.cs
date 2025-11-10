using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace OutputCachingBasics.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CacheTestsController : ControllerBase
{
    private static object Payload(string name, HttpContext ctx)
    {
        return new
        {
            Name = name,
            Time = DateTime.UtcNow,
            Auth = ctx.Request.Headers["Authorization"].ToString(),
            TraceId = ctx.TraceIdentifier,
        };
    } 
    
    [OutputCache]
    [HttpGet("base-policy")]
    public IActionResult BasePolicy() => Ok(Payload("BasePolicy", HttpContext));

    [OutputCache(PolicyName = "CacheForOneMinute")]
    [HttpGet("minute")]
    public IActionResult CacheForOneMinute() => Ok(Payload("CacheForOneMinute", HttpContext));
    
    [OutputCache(PolicyName = "NoCache")]
    [HttpGet("no-cache")]
    public IActionResult NoCache() => Ok(Payload("NoCache", HttpContext));
    
    [OutputCache(PolicyName = "AuthenticatedUserCachePolicy")]
    [HttpGet("auth-vary")]
    public IActionResult AuthVary() => Ok(Payload("AuthenticatedUserCachePolicy", HttpContext));
    
    
}