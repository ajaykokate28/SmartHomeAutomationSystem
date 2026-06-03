using Microsoft.AspNetCore.Mvc;
using SmartHomeAutomationSystem.Services.Interfaces;

namespace SmartHomeAutomationSystem.Api.Controllers;

/// <summary>Endpoints for reports and ML.NET anomaly analysis.</summary>
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly IAnomalyDetectionService _anomalyService;

    public ReportsController(IReportService reportService, IAnomalyDetectionService anomalyService)
    {
        _reportService = reportService;
        _anomalyService = anomalyService;
    }

    /// <summary>Returns the device usage report.</summary>
    [HttpGet("devices")]
    public IActionResult DeviceUsage() => Ok(_reportService.GetDeviceUsageReport());

    /// <summary>Returns the automation rules report.</summary>
    [HttpGet("rules")]
    public IActionResult AutomationRules() => Ok(_reportService.GetAutomationRuleReport());

    /// <summary>Returns the user activity report.</summary>
    [HttpGet("users")]
    public IActionResult UserActivity() => Ok(_reportService.GetUserActivityReport());

    /// <summary>Returns all raw device event logs recorded for anomaly detection.</summary>
    [HttpGet("anomaly/events")]
    public IActionResult AnomalyEvents() => Ok(_anomalyService.GetAllEvents());

    /// <summary>Runs ML.NET spike detection for a specific device.</summary>
    [HttpGet("anomaly/spikes/{deviceId:guid}")]
    public IActionResult Spikes(Guid deviceId) => Ok(_anomalyService.DetectSpikes(deviceId));

    /// <summary>Runs ML.NET change-point detection for a specific device.</summary>
    [HttpGet("anomaly/changepoints/{deviceId:guid}")]
    public IActionResult ChangePoints(Guid deviceId) => Ok(_anomalyService.DetectChangePoints(deviceId));
}
