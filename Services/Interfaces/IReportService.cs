using SmartHomeAutomationSystem.Models;

namespace SmartHomeAutomationSystem.Services.Interfaces;

/// <summary>
/// Defines operations for generating system summary reports.
/// </summary>
public interface IReportService
{
    /// <summary>Generates and prints a report of all devices and their current status.</summary>
    void GenerateDeviceUsageReport();

    /// <summary>Generates and prints a report of all automation rules.</summary>
    void GenerateAutomationRuleReport();

    /// <summary>Generates and prints a report of all registered users and their roles.</summary>
    void GenerateUserActivityReport();

    /// <summary>Returns device usage data as a structured report object.</summary>
    Report GetDeviceUsageReport();

    /// <summary>Returns automation rules data as a structured report object.</summary>
    Report GetAutomationRuleReport();

    /// <summary>Returns user activity data as a structured report object.</summary>
    Report GetUserActivityReport();
}
