using Moq;
using SmartHomeAutomationSystem.Models;
using SmartHomeAutomationSystem.Patterns.Repository;
using SmartHomeAutomationSystem.Services;
using SmartHomeAutomationSystem.Services.Interfaces;
using Xunit;

namespace SmartHomeAutomationSystem.Tests.Services;

/// <summary>
/// Unit tests for <see cref="AutomationRuleService"/>.
/// </summary>
public class AutomationRuleServiceTests : IDisposable
{
    private readonly Mock<IRepository<AutomationRule>> _repositoryMock;
    private readonly AutomationRuleService _sut;

    public AutomationRuleServiceTests()
    {
        _repositoryMock = new Mock<IRepository<AutomationRule>>();
        _sut = new AutomationRuleService(_repositoryMock.Object);
    }

    public void Dispose() => _sut.Dispose();

    // ──────────────────────────────────────────────────────────────────────────
    // CreateRule
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>CreateRule should call repository.Add once with the correct entity.</summary>
    [Fact]
    public void CreateRule_ShouldCallRepositoryAdd()
    {
        _sut.CreateRule("Motion Light", "motion detected", "turn on light");

        _repositoryMock.Verify(
            r => r.Add(It.Is<AutomationRule>(rule =>
                rule.Name == "Motion Light" &&
                rule.Condition == "motion detected" &&
                rule.Action == "turn on light" &&
                rule.Schedule == null)),
            Times.Once);
    }

    /// <summary>CreateRule with a schedule should persist the schedule value.</summary>
    [Fact]
    public void CreateRule_WithSchedule_ShouldPersistSchedule()
    {
        _sut.CreateRule("Night Lights Off", "time check", "turn off lights", "22:00");

        _repositoryMock.Verify(
            r => r.Add(It.Is<AutomationRule>(rule => rule.Schedule == "22:00")),
            Times.Once);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // UpdateRule
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>UpdateRule should call repository.Update with correct values when rule exists.</summary>
    [Fact]
    public void UpdateRule_WhenRuleExists_ShouldCallRepositoryUpdate()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.Update(id, It.IsAny<AutomationRule>())).Returns(true);

        _sut.UpdateRule(id, "Updated Rule", "motion detected", "turn off light");

        _repositoryMock.Verify(
            r => r.Update(id, It.Is<AutomationRule>(rule => rule.Name == "Updated Rule")),
            Times.Once);
    }

    /// <summary>UpdateRule should still call repository.Update even when rule is not found.</summary>
    [Fact]
    public void UpdateRule_WhenRuleNotFound_ShouldCallRepositoryUpdateOnce()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.Update(id, It.IsAny<AutomationRule>())).Returns(false);

        _sut.UpdateRule(id, "Ghost Rule", "none", "none");

        _repositoryMock.Verify(r => r.Update(id, It.IsAny<AutomationRule>()), Times.Once);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // DeleteRule
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>DeleteRule should call repository.Delete with the correct id.</summary>
    [Fact]
    public void DeleteRule_WhenRuleExists_ShouldCallRepositoryDelete()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.Delete(id)).Returns(true);

        _sut.DeleteRule(id);

        _repositoryMock.Verify(r => r.Delete(id), Times.Once);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // GetAllRules / GetRuleById
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>GetAllRules should return all rules from the repository.</summary>
    [Fact]
    public void GetAllRules_ShouldReturnAllRulesFromRepository()
    {
        var rules = new List<AutomationRule>
        {
            new() { Name = "Rule 1", Condition = "motion detected", Action = "turn on light" },
            new() { Name = "Rule 2", Condition = "temperature high", Action = "turn on fan" }
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(rules);

        var result = _sut.GetAllRules();

        Assert.Equal(2, result.Count);
    }

    /// <summary>GetRuleById should delegate to repository.GetById.</summary>
    [Fact]
    public void GetRuleById_ShouldReturnRuleFromRepository()
    {
        var id = Guid.NewGuid();
        var rule = new AutomationRule { Id = id, Name = "Rule", Condition = "cond", Action = "act" };
        _repositoryMock.Setup(r => r.GetById(id)).Returns(rule);

        var result = _sut.GetRuleById(id);

        Assert.NotNull(result);
        Assert.Equal("Rule", result.Name);
    }

    /// <summary>GetRuleById should return null when rule does not exist.</summary>
    [Fact]
    public void GetRuleById_WhenNotFound_ShouldReturnNull()
    {
        _repositoryMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((AutomationRule?)null);

        var result = _sut.GetRuleById(Guid.NewGuid());

        Assert.Null(result);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // ExecuteRules
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>ExecuteRules should update a light device when "motion detected" condition matches.</summary>
    [Fact]
    public void ExecuteRules_MotionDetected_ShouldUpdateLightDevice()
    {
        var rule = new AutomationRule
        {
            Name = "Motion Light",
            Condition = "motion detected",
            Action = "turn on light"
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(new List<AutomationRule> { rule });

        var lightId = Guid.NewGuid();
        var deviceServiceMock = new Mock<IDeviceService>();
        deviceServiceMock.Setup(d => d.GetAllDevices()).Returns(new List<Device>
        {
            new() { Id = lightId, Name = "Living Room Light", Type = "Light", Status = "Off" }
        });

        _sut.ExecuteRules(deviceServiceMock.Object);

        deviceServiceMock.Verify(
            d => d.UpdateDevice(lightId, "Living Room Light", "Light", "On"),
            Times.Once);
    }

    /// <summary>ExecuteRules should not update any device when no condition matches.</summary>
    [Fact]
    public void ExecuteRules_NoMatchingCondition_ShouldNotUpdateAnyDevice()
    {
        var rule = new AutomationRule
        {
            Name = "Unmatched Rule",
            Condition = "temperature above 30",
            Action = "turn on fan"
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(new List<AutomationRule> { rule });

        var deviceServiceMock = new Mock<IDeviceService>();
        deviceServiceMock.Setup(d => d.GetAllDevices()).Returns(new List<Device>());

        _sut.ExecuteRules(deviceServiceMock.Object);

        deviceServiceMock.Verify(d => d.UpdateDevice(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }
}
