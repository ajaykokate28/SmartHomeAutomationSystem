using SmartHomeAutomationSystem.Models;
using SmartHomeAutomationSystem.Patterns.PubSub;

namespace SmartHomeAutomationSystem.Services;

/// <summary>
/// Concrete subscriber that prints notifications to the console.
/// Implements <see cref="INotificationSubscriber"/> and participates in the EventBus Pub/Sub flow.
/// </summary>
public class NotificationService : INotificationSubscriber
{
    /// <inheritdoc/>
    public void Receive(Notification notification)
    {
        Console.WriteLine($"[Notification] {notification}");
    }
}

