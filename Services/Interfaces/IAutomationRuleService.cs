using SmartHomeAutomationSystem.Models;

namespace SmartHomeAutomationSystem.Services.Interfaces;

/// <summary>
/// Defines operations for managing and executing smart home automation rules.
/// </summary>
public interface IAutomationRuleService
{
    /// <summary>Creates a new automation rule.</summary>
    /// <param name="name">The rule name.</param>
    /// <param name="condition">The trigger condition (e.g. "motion detected").</param>
    /// <param name="action">The action to perform (e.g. "turn on light").</param>
    /// <param name="schedule">Optional HH:mm schedule time; <c>null</c> for event-driven rules.</param>
    void CreateRule(string name, string condition, string action, string? schedule = null);

    /// <summary>Updates an existing rule identified by <paramref name="id"/>.</summary>
    void UpdateRule(Guid id, string name, string condition, string action, string? schedule = null);

    /// <summary>Deletes the rule with the specified <paramref name="id"/>.</summary>
    void DeleteRule(Guid id);

    /// <summary>Writes all automation rules to the console.</summary>
    void ViewRules();

    /// <summary>Returns all automation rules.</summary>
    List<AutomationRule> GetAllRules();

    /// <summary>Returns the rule with the specified <paramref name="id"/>, or <c>null</c> if not found.</summary>
    AutomationRule? GetRuleById(Guid id);

    /// <summary>Evaluates all rules against the current state and executes matching actions.</summary>
    /// <param name="deviceService">The device service used to apply rule actions.</param>
    void ExecuteRules(IDeviceService deviceService);

    /// <summary>Starts a background scheduler that periodically checks time-based rules.</summary>
    /// <param name="deviceService">The device service used to apply scheduled actions.</param>
    void StartScheduler(IDeviceService deviceService);
}
