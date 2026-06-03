using Microsoft.AspNetCore.Mvc;
using SmartHomeAutomationSystem.Services.Interfaces;

namespace SmartHomeAutomationSystem.Api.Controllers;

/// <summary>CRUD endpoints for system users.</summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
        => _userService = userService;

    /// <summary>Returns all registered users.</summary>
    [HttpGet]
    public IActionResult GetAll() => Ok(_userService.GetAllUsers());

    /// <summary>Registers a new user.</summary>
    [HttpPost]
    public IActionResult Register([FromBody] CreateUserRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name) || string.IsNullOrWhiteSpace(req.Role))
            return BadRequest("Name and Role are required.");

        try
        {
            _userService.RegisterUser(req.Name, req.Role);
            return Ok(new { message = "User registered." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>Updates a user's name by ID.</summary>
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UpdateUserRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name))
            return BadRequest("Name is required.");

        _userService.UpdateUser(id, req.Name);
        return Ok(new { message = "User updated." });
    }

    /// <summary>Deletes a user by ID.</summary>
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        _userService.DeleteUser(id);
        return Ok(new { message = "User deleted." });
    }
}

/// <summary>Request body for user registration.</summary>
public record CreateUserRequest(string Name, string Role);

/// <summary>Request body for user name update.</summary>
public record UpdateUserRequest(string Name);
