namespace SmartHomeAutomationSystem.Models;

/// <summary>
/// Represents a single timestamped event recorded when a device status changes.
/// Used as the ML.NET input schema for anomaly detection training and scoring.
/// </summary>
public class DeviceEventLog
{
    /// <summary>Gets or sets the device identifier this event belongs to.</summary>
    public Guid DeviceId { get; set; }

    /// <summary>Gets or sets the name of the device.</summary>
    public required string DeviceName { get; set; }

    /// <summary>Gets or sets the device type (e.g. Light, Thermostat, Camera).</summary>
    public required string DeviceType { get; set; }

    /// <summary>Gets or sets the encoded numeric status value used as the ML.NET feature.
    /// On = 1, Off = 0, Idle = 0.5, other = 0.</summary>
    public float StatusValue { get; set; }

    /// <summary>Gets or sets the UTC timestamp of the event (stored as Unix seconds for ML use).</summary>
    public float HourOfDay { get; set; }

    /// <summary>Gets or sets the raw status string (e.g. On, Off, Idle).</summary>
    public required string Status { get; set; }

    /// <summary>Gets or sets when this event was recorded.</summary>
    public DateTime RecordedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Converts a status string to a numeric value for use as an ML.NET feature.
    /// </summary>
    public static float EncodeStatus(string status) => status.ToLower() switch
    {
        "on"   => 1.0f,
        "idle" => 0.5f,
        _      => 0.0f  // Off or unknown
    };
}
