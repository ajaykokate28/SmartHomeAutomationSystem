using SmartHomeAutomationSystem.Models;

namespace SmartHomeAutomationSystem.Services.Interfaces;

/// <summary>
/// Defines CRUD operations and query access for smart home devices.
/// </summary>
public interface IDeviceService
{
    /// <summary>Adds a new device to the system.</summary>
    /// <param name="name">The device name.</param>
    /// <param name="type">The device type (e.g. Light, Thermostat).</param>
    /// <param name="status">The initial status (e.g. On, Off, Idle).</param>
    void AddDevice(string name, string type, string status);

    /// <summary>Updates an existing device identified by <paramref name="id"/>.</summary>
    /// <param name="id">The unique identifier of the device to update.</param>
    /// <param name="name">The new device name.</param>
    /// <param name="type">The new device type.</param>
    /// <param name="status">The new device status.</param>
    void UpdateDevice(Guid id, string name, string type, string status);

    /// <summary>Deletes the device with the specified <paramref name="id"/>.</summary>
    /// <param name="id">The unique identifier of the device to delete.</param>
    void DeleteDevice(Guid id);

    /// <summary>Writes all devices to the console.</summary>
    void ViewDevices();

    /// <summary>Returns all registered devices.</summary>
    List<Device> GetAllDevices();
}
