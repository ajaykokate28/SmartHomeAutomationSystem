namespace SmartHomeAutomationSystem.Patterns.PubSub;

/// <summary>
/// Convenience facade over EventBus for managing subscriptions.
/// Demonstrates the Facade pattern — simplifies subscribe/unsubscribe lifecycle.
/// </summary>
public class PubSubManager
{
    private readonly EventBus _eventBus;
    private readonly List<INotificationSubscriber> _managedSubscribers = new();

    /// <summary>Initialises a new <see cref="PubSubManager"/> that manages subscriptions on the given event bus.</summary>
    /// <param name="eventBus">The event bus to wrap.</param>
    public PubSubManager(EventBus eventBus)
    {
        _eventBus = eventBus;
    }

    /// <summary>Subscribes a new subscriber and tracks it for bulk deregistration.</summary>
    /// <param name="subscriber">The subscriber to register.</param>
    public void Register(INotificationSubscriber subscriber)
    {
        _eventBus.Subscribe(subscriber);
        _managedSubscribers.Add(subscriber);
    }

    /// <summary>Removes a specific subscriber.</summary>
    public void Deregister(INotificationSubscriber subscriber)
    {
        _eventBus.Unsubscribe(subscriber);
        _managedSubscribers.Remove(subscriber);
    }

    /// <summary>Unsubscribes all managed subscribers at once.</summary>
    public void DeregisterAll()
    {
        foreach (var subscriber in _managedSubscribers)
            _eventBus.Unsubscribe(subscriber);

        _managedSubscribers.Clear();
    }
}
