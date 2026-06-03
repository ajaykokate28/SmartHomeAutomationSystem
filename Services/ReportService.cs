using SmartHomeAutomationSystem.Models;
using SmartHomeAutomationSystem.Services.Interfaces;

namespace SmartHomeAutomationSystem.Services;

/// <summary>
/// Generates textual summary reports for devices, automation rules, and users.
/// Depends on service interfaces (DIP) to remain decoupled from concrete implementations.
/// </summary>
public class ReportService : IReportService
{
    private readonly IDeviceService _deviceService;
    private readonly IAutomationRuleService _ruleService;
    private readonly IUserService _userService;

    /// <summary>Initialises a new <see cref="ReportService"/> with the required service dependencies.</summary>
    /// <param name="deviceService">Service providing device data.</param>
    /// <param name="ruleService">Service providing automation rule data.</param>
    /// <param name="userService">Service providing user data.</param>
    public ReportService(
        IDeviceService deviceService,
        IAutomationRuleService ruleService,
        IUserService userService)
    {
        _deviceService = deviceService;
        _ruleService = ruleService;
        _userService = userService;
    }

    /// <inheritdoc/>
    public void GenerateDeviceUsageReport()
    {
        var devices = _deviceService.GetAllDevices();
        var content = string.Join("\n", devices.Select(d => $"{d.Name} ({d.Type}) - Status: {d.Status}"));
        var report = new Report { Type = "Device Usage", Content = content };
        Console.WriteLine(report);
    }

    /// <inheritdoc/>
    public void GenerateAutomationRuleReport()
    {
        var rules = _ruleService.GetAllRules();
        var content = string.Join("\n", rules.Select(r => $"{r.Name} - Condition: {r.Condition}, Action: {r.Action}, Schedule: {r.Schedule}"));
        var report = new Report { Type = "Automation Rules", Content = content };
        Console.WriteLine(report);
    }

    /// <inheritdoc/>
    public void GenerateUserActivityReport()
    {
        var users = _userService.GetAllUsers();
        var content = string.Join("\n", users.Select(u => $"{u.Name} - Role: {u.Role}"));
        var report = new Report { Type = "User Activity", Content = content };
        Console.WriteLine(report);
    }

    /// <inheritdoc/>
    public Report GetDeviceUsageReport()
    {
        var devices = _deviceService.GetAllDevices();
        var content = string.Join("\n", devices.Select(d => $"{d.Name} ({d.Type}) - Status: {d.Status}"));
        return new Report { Type = "Device Usage", Content = content };
    }

    /// <inheritdoc/>
    public Report GetAutomationRuleReport()
    {
        var rules = _ruleService.GetAllRules();
        var content = string.Join("\n", rules.Select(r => $"{r.Name} - Condition: {r.Condition}, Action: {r.Action}, Schedule: {r.Schedule}"));
        return new Report { Type = "Automation Rules", Content = content };
    }

    /// <inheritdoc/>
    public Report GetUserActivityReport()
    {
        var users = _userService.GetAllUsers();
        var content = string.Join("\n", users.Select(u => $"{u.Name} - Role: {u.Role}"));
        return new Report { Type = "User Activity", Content = content };
    }
}

