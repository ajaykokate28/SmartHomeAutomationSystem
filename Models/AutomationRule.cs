namespace SmartHomeAutomationSystem.Models;

/// <summary>
/// Represents an automation rule that triggers an action when a condition is met,
/// optionally on a scheduled time.
/// </summary>
public class AutomationRule
{
    /// <summary>Gets or sets the unique identifier for this rule.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the name of the automation rule.</summary>
    public required string Name { get; set; }

    /// <summary>Gets or sets the trigger condition (e.g. "motion detected").</summary>
    public required string Condition { get; set; } // e.g., "motion detected"

    /// <summary>Gets or sets the action to perform when the condition is met (e.g. "turn on light").</summary>
    public required string Action { get; set; }    // e.g., "turn on light"

    /// <summary>Gets or sets an optional scheduled time in HH:mm format, or <c>null</c> for event-driven rules.</summary>
    public string? Schedule { get; set; }          // e.g., "22:00" or null

    /// <summary>Returns a formatted string representation of the automation rule.</summary>
    public override string ToString()
    {
        return $"ID: {Id}, Name: {Name}, Condition: {Condition}, Action: {Action}, Schedule: {Schedule}";
    }
}
