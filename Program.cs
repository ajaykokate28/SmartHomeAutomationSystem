// Program.cs — Smart Home Automation System entry point
using SmartHomeAutomationSystem.Patterns.PubSub;
using SmartHomeAutomationSystem.Patterns.Repository;
using SmartHomeAutomationSystem.Services;
using SmartHomeAutomationSystem.Services.Interfaces;
using SmartHomeAutomationSystem.Models;

// ── Dependency composition (manual DI, satisfies DIP) ────────────────────────
IAnomalyDetectionService anomalyService = new AnomalyDetectionService();
IDeviceService deviceService = new DeviceService(new DeviceRepository(), anomalyService);
IUserService userService = new UserService(new UserRepository());
IAutomationRuleService ruleService = new AutomationRuleService(new AutomationRuleRepository());

// ── Pub/Sub: register notification subscriber ────────────────────────────────
var notificationService = new NotificationService();
EventBus.Instance.Subscribe(notificationService);

// ── Device CRUD demo ─────────────────────────────────────────────────────────
Console.WriteLine("=== Device Management ===");
deviceService.AddDevice("Living Room Light", "Light", "Off");
deviceService.AddDevice("Thermostat", "Thermostat", "Idle");
deviceService.ViewDevices();

var firstDeviceId = deviceService.GetAllDevices().First().Id;
deviceService.UpdateDevice(firstDeviceId, "Living Room Light", "Light", "On");
deviceService.DeleteDevice(firstDeviceId);
deviceService.ViewDevices();

// ── User CRUD demo ────────────────────────────────────────────────────────────
Console.WriteLine("\n=== User Management ===");
userService.RegisterUser("Ajay", "Admin");
userService.RegisterUser("Vijay", "Homeowner");
userService.ViewUsers();

var firstUserId = userService.GetAllUsers().First().Id;
userService.UpdateUser(firstUserId, "Ajay Updated");
userService.DeleteUser(firstUserId);
userService.ViewUsers();

// ── Automation Rule + Scheduler demo ─────────────────────────────────────────
Console.WriteLine("\n=== Automation Rules ===");
deviceService.AddDevice("Bedroom Thermostat", "Thermostat", "On");

var nextMinute = DateTime.Now.AddMinutes(1).ToString("HH:mm");
ruleService.CreateRule("Turn Off Thermostat", $"time is {nextMinute}", "turn off thermostat", nextMinute);
ruleService.StartScheduler(deviceService);

Console.WriteLine("Press Enter to stop the scheduler and generate reports...");
Console.ReadLine();

// ── Report demo ───────────────────────────────────────────────────────────────
Console.WriteLine("\n=== Reports ===");
IReportService reportService = new ReportService(deviceService, ruleService, userService);
deviceService.AddDevice("Garden Light", "Light", "On");
ruleService.CreateRule("Motion Light", "motion detected", "turn on light");
userService.RegisterUser("Ajay", "Admin");

reportService.GenerateDeviceUsageReport();
reportService.GenerateAutomationRuleReport();
reportService.GenerateUserActivityReport();

// ── ML.NET Anomaly Detection demo ────────────────────────────────────────────
Console.WriteLine("\n=== ML.NET Anomaly Detection ===");
deviceService.AddDevice("ML Sensor", "Sensor", "Off");
var mlDeviceId = deviceService.GetAllDevices().Last().Id;

// Simulate a normal usage pattern (Off → On cycling)
var normalStatuses = new[] { "On", "Off", "On", "Off", "On", "Off", "On", "Off" };
foreach (var s in normalStatuses)
    deviceService.UpdateDevice(mlDeviceId, "ML Sensor", "Sensor", s);

// Inject two anomalous rapid-fire Idle spikes
deviceService.UpdateDevice(mlDeviceId, "ML Sensor", "Sensor", "Idle");
deviceService.UpdateDevice(mlDeviceId, "ML Sensor", "Sensor", "Idle");

// Run full ML analysis across all tracked devices
anomalyService.RunFullAnalysis();

// ── Cleanup disposable resources ─────────────────────────────────────────────
if (ruleService is IDisposable disposable)
    disposable.Dispose();






