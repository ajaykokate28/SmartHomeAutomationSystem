using Moq;
using SmartHomeAutomationSystem.Models;
using SmartHomeAutomationSystem.Services;
using SmartHomeAutomationSystem.Services.Interfaces;
using Xunit;

namespace SmartHomeAutomationSystem.Tests.Services;

/// <summary>
/// Unit tests for <see cref="ReportService"/>.
/// </summary>
public class ReportServiceTests
{
    private readonly Mock<IDeviceService> _deviceServiceMock;
    private readonly Mock<IAutomationRuleService> _ruleServiceMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly ReportService _sut;

    public ReportServiceTests()
    {
        _deviceServiceMock = new Mock<IDeviceService>();
        _ruleServiceMock = new Mock<IAutomationRuleService>();
        _userServiceMock = new Mock<IUserService>();
        _sut = new ReportService(_deviceServiceMock.Object, _ruleServiceMock.Object, _userServiceMock.Object);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // GenerateDeviceUsageReport
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>GenerateDeviceUsageReport should call GetAllDevices on the device service.</summary>
    [Fact]
    public void GenerateDeviceUsageReport_ShouldCallGetAllDevices()
    {
        _deviceServiceMock.Setup(d => d.GetAllDevices()).Returns(new List<Device>
        {
            new() { Name = "Light", Type = "Light", Status = "On" }
        });

        _sut.GenerateDeviceUsageReport();

        _deviceServiceMock.Verify(d => d.GetAllDevices(), Times.Once);
    }

    /// <summary>GenerateDeviceUsageReport with an empty device list should still succeed.</summary>
    [Fact]
    public void GenerateDeviceUsageReport_WhenNoDevices_ShouldNotThrow()
    {
        _deviceServiceMock.Setup(d => d.GetAllDevices()).Returns(new List<Device>());

        var exception = Record.Exception(() => _sut.GenerateDeviceUsageReport());

        Assert.Null(exception);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // GenerateAutomationRuleReport
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>GenerateAutomationRuleReport should call GetAllRules on the rule service.</summary>
    [Fact]
    public void GenerateAutomationRuleReport_ShouldCallGetAllRules()
    {
        _ruleServiceMock.Setup(r => r.GetAllRules()).Returns(new List<AutomationRule>
        {
            new() { Name = "Motion Light", Condition = "motion detected", Action = "turn on light" }
        });

        _sut.GenerateAutomationRuleReport();

        _ruleServiceMock.Verify(r => r.GetAllRules(), Times.Once);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // GenerateUserActivityReport
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>GenerateUserActivityReport should call GetAllUsers on the user service.</summary>
    [Fact]
    public void GenerateUserActivityReport_ShouldCallGetAllUsers()
    {
        _userServiceMock.Setup(u => u.GetAllUsers()).Returns(new List<User>
        {
            new AdminUser { Name = "Ajay" }
        });

        _sut.GenerateUserActivityReport();

        _userServiceMock.Verify(u => u.GetAllUsers(), Times.Once);
    }
}
