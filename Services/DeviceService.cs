using SmartHomeAutomationSystem.Models;
using SmartHomeAutomationSystem.Patterns.PubSub;
using SmartHomeAutomationSystem.Patterns.Repository;
using SmartHomeAutomationSystem.Services.Interfaces;

namespace SmartHomeAutomationSystem.Services;

/// <summary>
/// Manages smart home device lifecycle: add, update, delete, and query.
/// Publishes <see cref="Notification"/> events via <see cref="EventBus"/> on device state changes.
/// Optionally records device events for ML.NET anomaly detection.
/// </summary>
public class DeviceService : IDeviceService
{
    private readonly IRepository<Device> _repository;
    private readonly IAnomalyDetectionService? _anomalyDetectionService;

    /// <summary>Initialises a new instance with the specified device repository.</summary>
    /// <param name="repository">The device repository to use for persistence.</param>
    /// <param name="anomalyDetectionService">Optional anomaly detection service; pass <c>null</c> to disable ML tracking.</param>
    public DeviceService(IRepository<Device> repository, IAnomalyDetectionService? anomalyDetectionService = null)
    {
        _repository = repository;
        _anomalyDetectionService = anomalyDetectionService;
    }

    /// <inheritdoc/>
    public void AddDevice(string name, string type, string status)
    {
        var device = new Device { Name = name, Type = type, Status = status };
        _repository.Add(device);
        Console.WriteLine("Device added successfully.");
    }

    /// <inheritdoc/>
    public void UpdateDevice(Guid id, string name, string type, string status)
    {
        var updated = new Device { Name = name, Type = type, Status = status };
        if (_repository.Update(id, updated))
        {
            Console.WriteLine("Device updated.");
            EventBus.Instance.Publish(new Notification
            {
                Message = $"Device '{name}' status changed to '{status}'."
            });
            _anomalyDetectionService?.RecordEvent(updated);
        }
        else
        {
            Console.WriteLine("Device not found.");
        }
    }

    /// <inheritdoc/>
    public void DeleteDevice(Guid id)
    {
        if (_repository.Delete(id))
            Console.WriteLine("Device deleted.");
        else
            Console.WriteLine("Device not found.");
    }

    /// <inheritdoc/>
    public void ViewDevices()
    {
        var devices = _repository.GetAll();
        if (devices.Count == 0)
        {
            Console.WriteLine("No devices found.");
            return;
        }

        foreach (var device in devices)
            Console.WriteLine(device);
    }

    /// <inheritdoc/>
    public List<Device> GetAllDevices() => _repository.GetAll();
}

