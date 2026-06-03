using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using SmartHomeAutomationSystem.Models;
using SmartHomeAutomationSystem.Patterns.PubSub;
using SmartHomeAutomationSystem.Services.Interfaces;

namespace SmartHomeAutomationSystem.Services;

/// <summary>
/// Implements device anomaly detection using ML.NET's time-series algorithms:
/// <list type="bullet">
///   <item><see cref="DetectSpikes"/> — detects sudden, transient anomalies (e.g. thermostat unexpectedly turned on at 3am).</item>
///   <item><see cref="DetectChangePoints"/> — detects sustained shifts in device-usage patterns.</item>
/// </list>
/// Events are recorded via <see cref="RecordEvent"/> and stored in memory.
/// Anomaly alerts are published to the <see cref="EventBus"/> as <see cref="Notification"/> messages.
/// </summary>
public class AnomalyDetectionService : IAnomalyDetectionService
{
    private readonly MLContext _mlContext;
    private readonly List<DeviceEventLog> _eventLog = new();
    private readonly object _lock = new();

    // Minimum data points required before analysis is meaningful
    private const int MinDataPoints = 6;

    /// <summary>Initialises a new <see cref="AnomalyDetectionService"/> with a seeded <see cref="MLContext"/>.</summary>
    public AnomalyDetectionService()
    {
        _mlContext = new MLContext(seed: 42);
    }

    /// <inheritdoc/>
    public void RecordEvent(Device device)
    {
        ArgumentNullException.ThrowIfNull(device);

        var log = new DeviceEventLog
        {
            DeviceId   = device.Id,
            DeviceName = device.Name,
            DeviceType = device.Type,
            Status     = device.Status,
            StatusValue = DeviceEventLog.EncodeStatus(device.Status),
            HourOfDay  = DateTime.Now.Hour + DateTime.Now.Minute / 60.0f,
            RecordedAt = DateTime.Now
        };

        lock (_lock)
        {
            _eventLog.Add(log);
        }
    }

    /// <inheritdoc/>
    public List<AnomalyResult> DetectSpikes(Guid deviceId)
    {
        var events = GetEventsForDevice(deviceId);
        if (events.Count < MinDataPoints)
        {
            Console.WriteLine($"[AnomalyDetection] Not enough data for spike detection on device {deviceId} (need {MinDataPoints}, have {events.Count}).");
            return new List<AnomalyResult>();
        }

        var dataView = _mlContext.Data.LoadFromEnumerable(events);

        var pipeline = _mlContext.Transforms.DetectIidSpike(
            outputColumnName: "Prediction",
            inputColumnName:  nameof(DeviceEventLog.StatusValue),
            confidence:       95.0,
            pvalueHistoryLength: events.Count / 4);

        var model     = pipeline.Fit(dataView);
        var transformed = model.Transform(dataView);

        return ExtractResults(transformed, events, "Spike");
    }

    /// <inheritdoc/>
    public List<AnomalyResult> DetectChangePoints(Guid deviceId)
    {
        var events = GetEventsForDevice(deviceId);
        if (events.Count < MinDataPoints)
        {
            Console.WriteLine($"[AnomalyDetection] Not enough data for change-point detection on device {deviceId} (need {MinDataPoints}, have {events.Count}).");
            return new List<AnomalyResult>();
        }

        var dataView = _mlContext.Data.LoadFromEnumerable(events);

        var pipeline = _mlContext.Transforms.DetectIidChangePoint(
            outputColumnName: "Prediction",
            inputColumnName:  nameof(DeviceEventLog.StatusValue),
            confidence:       95.0,
            changeHistoryLength: events.Count / 4);

        var model       = pipeline.Fit(dataView);
        var transformed = model.Transform(dataView);

        return ExtractResults(transformed, events, "ChangePoint");
    }

    /// <inheritdoc/>
    public void RunFullAnalysis()
    {
        List<DeviceEventLog> snapshot;
        lock (_lock)
        {
            snapshot = new List<DeviceEventLog>(_eventLog);
        }

        var deviceIds = snapshot.Select(e => e.DeviceId).Distinct().ToList();

        if (deviceIds.Count == 0)
        {
            Console.WriteLine("[AnomalyDetection] No events recorded yet.");
            return;
        }

        Console.WriteLine("\n=== ML.NET Anomaly Detection Report ===");

        foreach (var deviceId in deviceIds)
        {
            var name = snapshot.First(e => e.DeviceId == deviceId).DeviceName;
            Console.WriteLine($"\n--- Device: {name} ---");

            var spikes = DetectSpikes(deviceId);
            var anomalousSpikes = spikes.Where(r => r.IsAnomaly).ToList();
            Console.WriteLine($"  Spikes detected: {anomalousSpikes.Count}");
            anomalousSpikes.ForEach(r => Console.WriteLine($"  {r}"));

            var changePoints = DetectChangePoints(deviceId);
            var anomalousChanges = changePoints.Where(r => r.IsAnomaly).ToList();
            Console.WriteLine($"  Change points detected: {anomalousChanges.Count}");
            anomalousChanges.ForEach(r => Console.WriteLine($"  {r}"));

            // Publish anomalies to EventBus
            foreach (var anomaly in anomalousSpikes.Concat(anomalousChanges))
            {
                EventBus.Instance.Publish(new Notification { Message = anomaly.Message });
            }
        }

        Console.WriteLine("\n=== End of Anomaly Detection Report ===\n");
    }

    /// <inheritdoc/>
    public List<DeviceEventLog> GetAllEvents()
    {
        lock (_lock)
        {
            return new List<DeviceEventLog>(_eventLog);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Private helpers
    // ──────────────────────────────────────────────────────────────────────────

    private List<DeviceEventLog> GetEventsForDevice(Guid deviceId)
    {
        lock (_lock)
        {
            return _eventLog.Where(e => e.DeviceId == deviceId).ToList();
        }
    }

    private List<AnomalyResult> ExtractResults(
        IDataView transformed,
        List<DeviceEventLog> events,
        string kind)
    {
        // ML.NET Prediction column = [alert(0/1), score, p-value]
        var predictions = _mlContext.Data
            .CreateEnumerable<DeviceEventPrediction>(transformed, reuseRowObject: false)
            .ToList();

        var results = new List<AnomalyResult>();

        for (int i = 0; i < predictions.Count && i < events.Count; i++)
        {
            var pred = predictions[i];
            bool isAnomaly = pred.Prediction[0] == 1;

            results.Add(new AnomalyResult
            {
                DeviceId   = events[i].DeviceId,
                DeviceName = events[i].DeviceName,
                IsAnomaly  = isAnomaly,
                Score      = pred.Prediction[1],
                PValue     = pred.Prediction[2],
                Message    = isAnomaly
                    ? $"[ML.NET {kind}] ⚠ Anomaly on '{events[i].DeviceName}' at {events[i].RecordedAt:HH:mm:ss}: status={events[i].Status}, score={pred.Prediction[1]:F3}"
                    : $"[ML.NET {kind}] Normal behaviour on '{events[i].DeviceName}' at {events[i].RecordedAt:HH:mm:ss}"
            });
        }

        return results;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Internal ML.NET output schema
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>ML.NET output schema: Prediction[0]=alert, [1]=score, [2]=p-value.</summary>
    private sealed class DeviceEventPrediction
    {
        [Microsoft.ML.Data.VectorType(3)]
        public double[] Prediction { get; set; } = Array.Empty<double>();
    }
}
