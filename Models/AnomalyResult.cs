namespace SmartHomeAutomationSystem.Models;

/// <summary>
/// Holds the result of an ML.NET anomaly detection check for a single device event.
/// </summary>
public class AnomalyResult
{
    /// <summary>Gets or sets the device identifier.</summary>
    public Guid DeviceId { get; set; }

    /// <summary>Gets or sets the device name.</summary>
    public required string DeviceName { get; set; }

    /// <summary>Gets or sets a value indicating whether an anomaly was detected.</summary>
    public bool IsAnomaly { get; set; }

    /// <summary>Gets or sets the raw anomaly score (higher = more anomalous).</summary>
    public double Score { get; set; }

    /// <summary>Gets or sets the p-value from the detection algorithm (lower = more anomalous).</summary>
    public double PValue { get; set; }

    /// <summary>Gets or sets a human-readable explanation of the anomaly.</summary>
    public required string Message { get; set; }

    /// <summary>Returns a formatted anomaly result string.</summary>
    public override string ToString() =>
        $"[Anomaly] Device: {DeviceName} | IsAnomaly: {IsAnomaly} | Score: {Score:F3} | PValue: {PValue:F4} | {Message}";
}
