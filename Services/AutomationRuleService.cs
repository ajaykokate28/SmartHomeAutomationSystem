using SmartHomeAutomationSystem.Models;
using SmartHomeAutomationSystem.Patterns.PubSub;
using SmartHomeAutomationSystem.Patterns.Repository;
using SmartHomeAutomationSystem.Services.Interfaces;

namespace SmartHomeAutomationSystem.Services;

/// <summary>
/// Manages automation rules and their execution.
/// Supports both event-driven conditions and time-scheduled rules via a background timer.
/// Implements <see cref="IDisposable"/> to cleanly stop the scheduler.
/// </summary>
public class AutomationRuleService : IAutomationRuleService, IDisposable
{
    private readonly IRepository<AutomationRule> _repository;
    private System.Timers.Timer? _timer;

    /// <summary>Initialises a new instance with the specified automation rule repository.</summary>
    /// <param name="repository">The repository to use for rule persistence.</param>
    public AutomationRuleService(IRepository<AutomationRule> repository)
    {
        _repository = repository;
    }

    /// <inheritdoc/>
    public void CreateRule(string name, string condition, string action, string? schedule = null)
    {
        var rule = new AutomationRule
        {
            Name = name,
            Condition = condition,
            Action = action,
            Schedule = schedule
        };
        _repository.Add(rule);
        Console.WriteLine("Automation rule created.");
    }

    /// <inheritdoc/>
    public void UpdateRule(Guid id, string name, string condition, string action, string? schedule = null)
    {
        var updated = new AutomationRule
        {
            Name = name,
            Condition = condition,
            Action = action,
            Schedule = schedule
        };

        if (_repository.Update(id, updated))
            Console.WriteLine("Rule updated.");
        else
            Console.WriteLine("Rule not found.");
    }

    /// <inheritdoc/>
    public void DeleteRule(Guid id)
    {
        if (_repository.Delete(id))
            Console.WriteLine("Rule deleted.");
        else
            Console.WriteLine("Rule not found.");
    }

    /// <inheritdoc/>
    public void ViewRules()
    {
        var rules = _repository.GetAll();
        if (rules.Count == 0)
        {
            Console.WriteLine("No automation rules found.");
            return;
        }

        foreach (var rule in rules)
            Console.WriteLine(rule);
    }

    /// <inheritdoc/>
    public List<AutomationRule> GetAllRules() => _repository.GetAll();

    /// <inheritdoc/>
    public AutomationRule? GetRuleById(Guid id) => _repository.GetById(id);

    /// <inheritdoc/>
    public void ExecuteRules(IDeviceService deviceService)
    {
        var rules = _repository.GetAll();
        foreach (var rule in rules)
        {
            if (rule.Condition.Contains("motion detected", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"[Rule Triggered] {rule.Name}: {rule.Condition} → {rule.Action}");

                var light = deviceService.GetAllDevices()
                    .FirstOrDefault(d => d.Type.Equals("Light", StringComparison.OrdinalIgnoreCase));

                if (light != null)
                {
                    deviceService.UpdateDevice(light.Id, light.Name, light.Type, "On");
                    EventBus.Instance.Publish(new Notification
                    {
                        Message = $"Rule '{rule.Name}' executed: {rule.Action}"
                    });
                }
            }

            if (!string.IsNullOrEmpty(rule.Schedule))
            {
                var currentTime = DateTime.Now.ToString("HH:mm");
                if (currentTime == rule.Schedule)
                {
                    Console.WriteLine($"[Scheduled Rule Triggered] {rule.Name} at {rule.Schedule}");

                    var thermostat = deviceService.GetAllDevices()
                        .FirstOrDefault(d => d.Type.Equals("thermostat", StringComparison.OrdinalIgnoreCase));

                    if (thermostat != null)
                    {
                        deviceService.UpdateDevice(thermostat.Id, thermostat.Name, thermostat.Type, "Off");
                        Console.WriteLine($"[Action] {thermostat.Name} turned OFF.");
                    }
                }
            }
        }
    }

    /// <inheritdoc/>
    public void StartScheduler(IDeviceService deviceService)
    {
        _timer = new System.Timers.Timer(5000); // Check every 5 seconds for demo
        _timer.Elapsed += (sender, e) => ExecuteScheduledRules(deviceService);
        _timer.AutoReset = true;
        _timer.Enabled = true;
        Console.WriteLine("Scheduler started. Checking for scheduled rules...");
    }

    private void ExecuteScheduledRules(IDeviceService deviceService)
    {
        var currentTime = DateTime.Now.ToString("HH:mm");
        var rules = _repository.GetAll();

        foreach (var rule in rules)
        {
            if (!string.IsNullOrEmpty(rule.Schedule) && rule.Schedule == currentTime)
            {
                Console.WriteLine($"[Scheduled Rule Triggered] {rule.Name} at {rule.Schedule}");

                var device = deviceService.GetAllDevices()
                    .FirstOrDefault(d => rule.Action.Contains(d.Type, StringComparison.OrdinalIgnoreCase));

                if (device != null)
                {
                    var newStatus = rule.Action.Contains("off", StringComparison.OrdinalIgnoreCase) ? "Off" : "On";
                    deviceService.UpdateDevice(device.Id, device.Name, device.Type, newStatus);
                    Console.WriteLine($"[Action] {device.Name} turned {newStatus.ToUpper()}.");
                }
            }
        }
    }

    /// <summary>Releases the background scheduler timer.</summary>
    public void Dispose()
    {
        _timer?.Stop();
        _timer?.Dispose();
    }
}

