using SmartHomeAutomationSystem.Models;

namespace SmartHomeAutomationSystem.Patterns.PubSub
{
    /// <summary>
    /// Defines the contract for objects that can receive <see cref="Notification"/> messages
    /// from the <see cref="EventBus"/> (Observer / Subscriber role in Pub/Sub pattern).
    /// </summary>
    public interface INotificationSubscriber
    {
        /// <summary>Called by the <see cref="EventBus"/> when a notification is published.</summary>
        /// <param name="notification">The notification that was published.</param>
        void Receive(Notification notification);
    }
}
