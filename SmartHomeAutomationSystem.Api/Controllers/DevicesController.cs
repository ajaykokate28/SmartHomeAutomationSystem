using Microsoft.AspNetCore.Mvc;
using SmartHomeAutomationSystem.Services.Interfaces;

namespace SmartHomeAutomationSystem.Api.Controllers;

/// <summary>CRUD endpoints for smart home devices.</summary>
[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DevicesController(IDeviceService deviceService)
        => _deviceService = deviceService;

    /// <summary>Returns all registered devices.</summary>
    [HttpGet]
    public IActionResult GetAll() => Ok(_deviceService.GetAllDevices());

    /// <summary>Adds a new device.</summary>
    [HttpPost]
    public IActionResult Create([FromBody] CreateDeviceRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name) || string.IsNullOrWhiteSpace(req.Type) || string.IsNullOrWhiteSpace(req.Status))
            return BadRequest("Name, Type and Status are required.");

        _deviceService.AddDevice(req.Name, req.Type, req.Status);
        return Ok(new { message = "Device added." });
    }

    /// <summary>Updates an existing device by ID.</summary>
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] CreateDeviceRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name) || string.IsNullOrWhiteSpace(req.Type) || string.IsNullOrWhiteSpace(req.Status))
            return BadRequest("Name, Type and Status are required.");

        _deviceService.UpdateDevice(id, req.Name, req.Type, req.Status);
        return Ok(new { message = "Device updated." });
    }

    /// <summary>Deletes a device by ID.</summary>
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        _deviceService.DeleteDevice(id);
        return Ok(new { message = "Device deleted." });
    }
}

/// <summary>Request body for create/update device operations.</summary>
public record CreateDeviceRequest(string Name, string Type, string Status);
