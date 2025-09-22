using Microsoft.AspNetCore.Mvc;
using StudentBloggAPI.Features.Users.Interfaces;

namespace StudentBloggAPI.Features.Users;

[ApiController]
[Route("api/v1/[controller]")]
public class UsersController(IUserService userService) 
    : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet]
    public async Task<ActionResult> GetPagedUsersAsync(
        int pageNumber = 1,
        int pageSize = 10)
    {
        var users = await _userService.GetPagedAsync(pageNumber, pageSize);
        return Ok(users);
    }
}