using Microsoft.AspNetCore.Mvc;
using SmartHomeAutomationSystem.Services.Interfaces;

namespace SmartHomeAutomationSystem.Api.Controllers;

/// <summary>CRUD endpoints for automation rules.</summary>
[ApiController]
[Route("api/[controller]")]
public class AutomationRulesController : ControllerBase
{
    private readonly IAutomationRuleService _ruleService;

    public AutomationRulesController(IAutomationRuleService ruleService)
        => _ruleService = ruleService;

    /// <summary>Returns all automation rules.</summary>
    [HttpGet]
    public IActionResult GetAll() => Ok(_ruleService.GetAllRules());

    /// <summary>Creates a new automation rule.</summary>
    [HttpPost]
    public IActionResult Create([FromBody] CreateRuleRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name) || string.IsNullOrWhiteSpace(req.Condition) || string.IsNullOrWhiteSpace(req.Action))
            return BadRequest("Name, Condition and Action are required.");

        _ruleService.CreateRule(req.Name, req.Condition, req.Action, req.Schedule);
        return Ok(new { message = "Rule created." });
    }

    /// <summary>Updates an existing automation rule by ID.</summary>
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] CreateRuleRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name) || string.IsNullOrWhiteSpace(req.Condition) || string.IsNullOrWhiteSpace(req.Action))
            return BadRequest("Name, Condition and Action are required.");

        _ruleService.UpdateRule(id, req.Name, req.Condition, req.Action, req.Schedule);
        return Ok(new { message = "Rule updated." });
    }

    /// <summary>Deletes an automation rule by ID.</summary>
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        _ruleService.DeleteRule(id);
        return Ok(new { message = "Rule deleted." });
    }
}

/// <summary>Request body for create/update automation rule.</summary>
public record CreateRuleRequest(string Name, string Condition, string Action, string? Schedule = null);
