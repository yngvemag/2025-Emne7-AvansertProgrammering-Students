using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentBloggAPI.Features.Users.Dtos;
using StudentBloggAPI.Features.Users.Interfaces;

namespace StudentBloggAPI.Features.Users;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class UsersController(ILogger<UsersController> logger, IUserService userService) 
    : ControllerBase
{
    private readonly ILogger<UsersController> _logger = logger;
    private readonly IUserService _userService = userService;

    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetPagedUsersAsync(
        int pageNumber = 1,
        int pageSize = 10)
    {
        var users = await _userService.GetPagedAsync(pageNumber, pageSize);
        return Ok(users);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserByIdAsync(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        return user is null
            ? NotFound("User not found")
            : Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> UpdateUserAsync(Guid id, UserDto user)
    {
        var updatedUser = await _userService.UpdateAsync(id, user);
        return updatedUser is null
            ? NotFound("User not found")
            : Ok(updatedUser);
    }
    
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> RegisterUserAsync(
        [FromBody] UserRegistrationDto userRegistrationDto)
    {
        UserDto? registeredUser = await _userService.RegisterAsync(userRegistrationDto);
        
        return registeredUser is null
            ? BadRequest("Failed to register user")
            : Ok(registeredUser);
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult<UserDto>> DeleteUserAsync(Guid id)
    {
        var deleteUser = await _userService.DeleteByIdAsync(id);
        return deleteUser is null
            ? NotFound("User not found")
            : Ok(deleteUser);
    }
}