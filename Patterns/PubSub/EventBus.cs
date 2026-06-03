using SmartHomeAutomationSystem.Models;

namespace SmartHomeAutomationSystem.Patterns.PubSub
{
    /// <summary>
    /// Thread-safe Singleton EventBus implementing the Publish/Subscribe pattern.
    /// </summary>
    public sealed class EventBus
    {
        private static readonly Lazy<EventBus> _instance = new(() => new EventBus());
        private readonly List<INotificationSubscriber> _subscribers = new();
        private readonly object _lock = new();

        private EventBus() { }

        /// <summary>Gets the single shared instance of <see cref="EventBus"/>.</summary>
        public static EventBus Instance => _instance.Value;

        /// <summary>Registers a subscriber to receive future notifications. Duplicate registrations are ignored.</summary>
        /// <param name="subscriber">The subscriber to register.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="subscriber"/> is <c>null</c>.</exception>
        public void Subscribe(INotificationSubscriber subscriber)
        {
            ArgumentNullException.ThrowIfNull(subscriber);
            lock (_lock)
            {
                if (!_subscribers.Contains(subscriber))
                    _subscribers.Add(subscriber);
            }
        }

        /// <summary>Removes a previously registered subscriber.</summary>
        /// <param name="subscriber">The subscriber to remove.</param>
        public void Unsubscribe(INotificationSubscriber subscriber)
        {
            lock (_lock)
            {
                _subscribers.Remove(subscriber);
            }
        }

        /// <summary>Publishes a notification to all currently registered subscribers.</summary>
        /// <param name="notification">The notification to broadcast.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="notification"/> is <c>null</c>.</exception>
        public void Publish(Notification notification)
        {
            ArgumentNullException.ThrowIfNull(notification);
            List<INotificationSubscriber> snapshot;
            lock (_lock)
            {
                snapshot = new List<INotificationSubscriber>(_subscribers);
            }

            foreach (var subscriber in snapshot)
                subscriber.Receive(notification);
        }
    }
}

