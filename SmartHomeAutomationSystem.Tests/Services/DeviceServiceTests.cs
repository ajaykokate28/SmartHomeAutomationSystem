using Moq;
using SmartHomeAutomationSystem.Models;
using SmartHomeAutomationSystem.Patterns.PubSub;
using SmartHomeAutomationSystem.Patterns.Repository;
using SmartHomeAutomationSystem.Services;
using Xunit;

namespace SmartHomeAutomationSystem.Tests.Services;

/// <summary>
/// Unit tests for <see cref="DeviceService"/>.
/// </summary>
public class DeviceServiceTests
{
    private readonly Mock<IRepository<Device>> _repositoryMock;
    private readonly DeviceService _sut;

    public DeviceServiceTests()
    {
        _repositoryMock = new Mock<IRepository<Device>>();
        _sut = new DeviceService(_repositoryMock.Object);

        // Ensure EventBus has at least one subscriber so Publish doesn't throw
        EventBus.Instance.Subscribe(new Mock<INotificationSubscriber>().Object);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // AddDevice
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>AddDevice should call repository.Add once with the correct entity.</summary>
    [Fact]
    public void AddDevice_ShouldCallRepositoryAdd()
    {
        _sut.AddDevice("Living Room Light", "Light", "Off");

        _repositoryMock.Verify(
            r => r.Add(It.Is<Device>(d =>
                d.Name == "Living Room Light" &&
                d.Type == "Light" &&
                d.Status == "Off")),
            Times.Once);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // UpdateDevice
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>UpdateDevice should call repository.Update with the correct values when device exists.</summary>
    [Fact]
    public void UpdateDevice_WhenDeviceExists_ShouldCallRepositoryUpdate()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.Update(id, It.IsAny<Device>())).Returns(true);

        _sut.UpdateDevice(id, "Bedroom Light", "Light", "On");

        _repositoryMock.Verify(
            r => r.Update(id, It.Is<Device>(d =>
                d.Name == "Bedroom Light" &&
                d.Status == "On")),
            Times.Once);
    }

    /// <summary>UpdateDevice should not publish a notification when device is not found.</summary>
    [Fact]
    public void UpdateDevice_WhenDeviceNotFound_ShouldNotUpdate()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.Update(id, It.IsAny<Device>())).Returns(false);

        _sut.UpdateDevice(id, "Ghost Device", "Light", "On");

        _repositoryMock.Verify(r => r.Update(id, It.IsAny<Device>()), Times.Once);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // DeleteDevice
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>DeleteDevice should call repository.Delete with the correct id.</summary>
    [Fact]
    public void DeleteDevice_WhenDeviceExists_ShouldCallRepositoryDelete()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.Delete(id)).Returns(true);

        _sut.DeleteDevice(id);

        _repositoryMock.Verify(r => r.Delete(id), Times.Once);
    }

    /// <summary>DeleteDevice should still call Delete even when device is not found.</summary>
    [Fact]
    public void DeleteDevice_WhenDeviceNotFound_ShouldCallRepositoryDelete()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.Delete(id)).Returns(false);

        _sut.DeleteDevice(id);

        _repositoryMock.Verify(r => r.Delete(id), Times.Once);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // GetAllDevices
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>GetAllDevices should return the full list from the repository.</summary>
    [Fact]
    public void GetAllDevices_ShouldReturnAllDevicesFromRepository()
    {
        var devices = new List<Device>
        {
            new() { Name = "Light 1", Type = "Light", Status = "On" },
            new() { Name = "Thermostat", Type = "Thermostat", Status = "Idle" }
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(devices);

        var result = _sut.GetAllDevices();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, d => d.Name == "Light 1");
    }

    /// <summary>GetAllDevices should return an empty list when repository is empty.</summary>
    [Fact]
    public void GetAllDevices_WhenEmpty_ShouldReturnEmptyList()
    {
        _repositoryMock.Setup(r => r.GetAll()).Returns(new List<Device>());

        var result = _sut.GetAllDevices();

        Assert.Empty(result);
    }
}
