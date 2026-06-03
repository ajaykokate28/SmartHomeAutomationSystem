using SmartHomeAutomationSystem.Models;

namespace SmartHomeAutomationSystem.Patterns.Repository
{
    /// <summary>In-memory <see cref="IRepository{T}"/> implementation for <see cref="Device"/> entities.</summary>
    public class DeviceRepository : IRepository<Device>
    {
        private readonly List<Device> _devices = new();

        /// <inheritdoc/>
        public void Add(Device device) => _devices.Add(device);

        /// <inheritdoc/>
        public bool Update(Guid id, Device updatedDevice)
        {
            var device = _devices.FirstOrDefault(d => d.Id == id);
            if (device == null) return false;

            device.Name = updatedDevice.Name;
            device.Type = updatedDevice.Type;
            device.Status = updatedDevice.Status;
            return true;
        }

        /// <inheritdoc/>
        public bool Delete(Guid id)
        {
            var device = _devices.FirstOrDefault(d => d.Id == id);
            return device != null && _devices.Remove(device);
        }

        /// <inheritdoc/>
        public List<Device> GetAll() => _devices;

        /// <inheritdoc/>
        public Device? GetById(Guid id) => _devices.FirstOrDefault(d => d.Id == id);
    }
}
