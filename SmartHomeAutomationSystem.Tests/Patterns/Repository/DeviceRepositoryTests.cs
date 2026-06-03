using SmartHomeAutomationSystem.Models;
using SmartHomeAutomationSystem.Patterns.Repository;
using Xunit;

namespace SmartHomeAutomationSystem.Tests.Patterns.Repository;

/// <summary>
/// Unit tests for <see cref="DeviceRepository"/>.
/// </summary>
public class DeviceRepositoryTests
{
    private DeviceRepository CreateRepository() => new();

    // ──────────────────────────────────────────────────────────────────────────
    // Add / GetAll
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>Add followed by GetAll should return the added device.</summary>
    [Fact]
    public void Add_ShouldMakeDeviceRetrievableViaGetAll()
    {
        var repo = CreateRepository();
        var device = new Device { Name = "Light", Type = "Light", Status = "Off" };

        repo.Add(device);

        Assert.Single(repo.GetAll());
        Assert.Contains(repo.GetAll(), d => d.Name == "Light");
    }

    /// <summary>GetAll on an empty repository should return an empty list.</summary>
    [Fact]
    public void GetAll_EmptyRepository_ShouldReturnEmptyList()
    {
        var repo = CreateRepository();

        Assert.Empty(repo.GetAll());
    }

    // ──────────────────────────────────────────────────────────────────────────
    // GetById
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>GetById should return the correct device when it exists.</summary>
    [Fact]
    public void GetById_WhenDeviceExists_ShouldReturnDevice()
    {
        var repo = CreateRepository();
        var device = new Device { Name = "Thermostat", Type = "Thermostat", Status = "Idle" };
        repo.Add(device);

        var result = repo.GetById(device.Id);

        Assert.NotNull(result);
        Assert.Equal("Thermostat", result.Name);
    }

    /// <summary>GetById should return null for an unknown id.</summary>
    [Fact]
    public void GetById_WhenDeviceNotFound_ShouldReturnNull()
    {
        var repo = CreateRepository();

        var result = repo.GetById(Guid.NewGuid());

        Assert.Null(result);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Update
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>Update should modify the existing device and return true.</summary>
    [Fact]
    public void Update_WhenDeviceExists_ShouldUpdateFields()
    {
        var repo = CreateRepository();
        var device = new Device { Name = "OldName", Type = "Light", Status = "Off" };
        repo.Add(device);

        var updated = new Device { Name = "NewName", Type = "Light", Status = "On" };
        var result = repo.Update(device.Id, updated);

        Assert.True(result);
        var stored = repo.GetById(device.Id);
        Assert.Equal("NewName", stored!.Name);
        Assert.Equal("On", stored.Status);
    }

    /// <summary>Update should return false when device does not exist.</summary>
    [Fact]
    public void Update_WhenDeviceNotFound_ShouldReturnFalse()
    {
        var repo = CreateRepository();
        var updated = new Device { Name = "X", Type = "Light", Status = "On" };

        var result = repo.Update(Guid.NewGuid(), updated);

        Assert.False(result);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Delete
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>Delete should remove the device and return true.</summary>
    [Fact]
    public void Delete_WhenDeviceExists_ShouldRemoveAndReturnTrue()
    {
        var repo = CreateRepository();
        var device = new Device { Name = "Camera", Type = "Camera", Status = "On" };
        repo.Add(device);

        var result = repo.Delete(device.Id);

        Assert.True(result);
        Assert.Empty(repo.GetAll());
    }

    /// <summary>Delete should return false when device does not exist.</summary>
    [Fact]
    public void Delete_WhenDeviceNotFound_ShouldReturnFalse()
    {
        var repo = CreateRepository();

        var result = repo.Delete(Guid.NewGuid());

        Assert.False(result);
    }
}
