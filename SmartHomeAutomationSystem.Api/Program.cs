using SmartHomeAutomationSystem.Patterns.Repository;
using SmartHomeAutomationSystem.Services;
using SmartHomeAutomationSystem.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ── Register application services (DI) ───────────────────────────────────────
builder.Services.AddSingleton<IAnomalyDetectionService, AnomalyDetectionService>();
builder.Services.AddSingleton<IRepository<SmartHomeAutomationSystem.Models.Device>, DeviceRepository>();
builder.Services.AddSingleton<IRepository<SmartHomeAutomationSystem.Models.User>, UserRepository>();
builder.Services.AddSingleton<IRepository<SmartHomeAutomationSystem.Models.AutomationRule>, AutomationRuleRepository>();
builder.Services.AddSingleton<IDeviceService>(sp =>
    new DeviceService(
        sp.GetRequiredService<IRepository<SmartHomeAutomationSystem.Models.Device>>(),
        sp.GetRequiredService<IAnomalyDetectionService>()));
builder.Services.AddSingleton<IUserService>(sp =>
    new UserService(sp.GetRequiredService<IRepository<SmartHomeAutomationSystem.Models.User>>()));
builder.Services.AddSingleton<IAutomationRuleService>(sp =>
    new AutomationRuleService(sp.GetRequiredService<IRepository<SmartHomeAutomationSystem.Models.AutomationRule>>()));
builder.Services.AddSingleton<IReportService>(sp =>
    new ReportService(
        sp.GetRequiredService<IDeviceService>(),
        sp.GetRequiredService<IAutomationRuleService>(),
        sp.GetRequiredService<IUserService>()));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Smart Home Automation API", Version = "v1" });
});

// ── CORS: allow Angular dev server (port 4200) ────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDev", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// Seed default records so the UI/API has visible data on startup.
SeedInitialData(app.Services);

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Home API v1"));

app.UseCors("AngularDev");
app.UseAuthorization();
app.MapControllers();

app.Run();

static void SeedInitialData(IServiceProvider services)
{
    var deviceService = services.GetRequiredService<IDeviceService>();
    var userService = services.GetRequiredService<IUserService>();
    var ruleService = services.GetRequiredService<IAutomationRuleService>();

    if (deviceService.GetAllDevices().Count == 0)
    {
        deviceService.AddDevice("Living Room Light", "Light", "Off");
        deviceService.AddDevice("Bedroom Thermostat", "Thermostat", "Idle");
        deviceService.AddDevice("Main Door Camera", "Camera", "On");
    }

    if (userService.GetAllUsers().Count == 0)
    {
        userService.RegisterUser("Ajay", "Admin");
        userService.RegisterUser("Vijay", "Homeowner");
    }

    if (ruleService.GetAllRules().Count == 0)
    {
        ruleService.CreateRule("Motion Light", "motion detected", "turn on light");
        ruleService.CreateRule("Night Thermostat Off", "time is 23:00", "turn off thermostat", "23:00");
    }
}
