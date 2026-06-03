using SmartHomeAutomationSystem.Models;

namespace SmartHomeAutomationSystem.Services.Interfaces;

/// <summary>
/// Defines operations for recording device events and detecting anomalies
/// using ML.NET's Spike Detection and Change-Point Detection algorithms.
/// </summary>
public interface IAnomalyDetectionService
{
    /// <summary>
    /// Records a device status change event into the in-memory event log.
    /// </summary>
    /// <param name="device">The device whose status has changed.</param>
    void RecordEvent(Device device);

    /// <summary>
    /// Analyses all recorded events for the specified device and returns
    /// anomaly results using ML.NET spike detection.
    /// </summary>
    /// <param name="deviceId">The device to analyse.</param>
    /// <returns>A list of <see cref="AnomalyResult"/> for each event window evaluated.</returns>
    List<AnomalyResult> DetectSpikes(Guid deviceId);

    /// <summary>
    /// Analyses all recorded events for the specified device and returns
    /// change-point results — sustained shifts in device behaviour.
    /// </summary>
    /// <param name="deviceId">The device to analyse.</param>
    /// <returns>A list of <see cref="AnomalyResult"/> for each change-point found.</returns>
    List<AnomalyResult> DetectChangePoints(Guid deviceId);

    /// <summary>
    /// Runs full anomaly analysis across all devices and prints a report to the console.
    /// </summary>
    void RunFullAnalysis();

    /// <summary>
    /// Returns all raw event logs currently recorded.
    /// </summary>
    List<DeviceEventLog> GetAllEvents();
}
