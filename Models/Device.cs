namespace SmartHomeAutomationSystem.Models;

/// <summary>
/// Represents a physical smart home device (e.g. light, thermostat, camera).
/// </summary>
public class Device
{
    /// <summary>Gets or sets the unique identifier for this device.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the human-readable name of the device.</summary>
    public required string Name { get; set; }

    /// <summary>Gets or sets the device type (e.g. Light, Thermostat, Camera).</summary>
    public required string Type { get; set; }  // Light, Thermostat, Camera, etc.

    /// <summary>Gets or sets the current operational status (e.g. On, Off, Idle).</summary>
    public required string Status { get; set; } // On, Off, Idle, etc.

    /// <summary>Returns a formatted string representation of the device.</summary>
    public override string ToString()
    {
        return $"ID: {Id}, Name: {Name}, Type: {Type}, Status: {Status}";
    }
}
