namespace SmartHomeAutomationSystem.Models;

/// <summary>
/// Represents a notification message published via the EventBus.
/// </summary>
public class Notification
{
    /// <summary>Gets or sets the notification message text.</summary>
    public required string Message { get; set; }

    /// <summary>Gets or sets the timestamp when the notification was created.</summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>Returns the notification formatted with its timestamp.</summary>
    public override string ToString()
    {
        return $"[{Timestamp:HH:mm:ss}] {Message}";
    }
}
