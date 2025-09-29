using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentBloggAPI.Features.Users.Dtos;
using StudentBloggAPI.Features.Users.Interfaces;

namespace StudentBloggAPI.Features.Users;

[Authorize]
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
    
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult> RegisterUserAsync(
        [FromBody] UserRegistrationDto userRegistrationDto)
    {
        UserDto? registeredUser = await _userService.RegisterAsync(userRegistrationDto);
        
        return registeredUser is null
            ? BadRequest("Failed to register user")
            : Ok(registeredUser);
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUserAsync(Guid id)
    {
        var deleteUser = await _userService.DeleteByIdAsync(id);
        return deleteUser is null
            ? NotFound("User not found")
            : Ok(deleteUser);
    }
}