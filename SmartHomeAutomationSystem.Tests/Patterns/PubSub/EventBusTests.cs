using Moq;
using SmartHomeAutomationSystem.Models;
using SmartHomeAutomationSystem.Patterns.PubSub;
using Xunit;

namespace SmartHomeAutomationSystem.Tests.Patterns.PubSub;

/// <summary>
/// Unit tests for <see cref="EventBus"/> (Singleton + Pub/Sub pattern).
/// </summary>
public class EventBusTests
{
    // ──────────────────────────────────────────────────────────────────────────
    // Singleton
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>EventBus.Instance should always return the same object (Singleton).</summary>
    [Fact]
    public void Instance_AcessedMultipleTimes_ShouldReturnSameObject()
    {
        var first = EventBus.Instance;
        var second = EventBus.Instance;

        Assert.Same(first, second);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Subscribe / Publish
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>Publish should call Receive on all subscribed subscribers.</summary>
    [Fact]
    public void Publish_WithSubscribers_ShouldCallReceiveOnEach()
    {
        var subscriber1 = new Mock<INotificationSubscriber>();
        var subscriber2 = new Mock<INotificationSubscriber>();

        EventBus.Instance.Subscribe(subscriber1.Object);
        EventBus.Instance.Subscribe(subscriber2.Object);

        var notification = new Notification { Message = "Test event" };
        EventBus.Instance.Publish(notification);

        subscriber1.Verify(s => s.Receive(notification), Times.AtLeastOnce);
        subscriber2.Verify(s => s.Receive(notification), Times.AtLeastOnce);
    }

    /// <summary>Subscribe should not add a duplicate subscriber.</summary>
    [Fact]
    public void Subscribe_SamSubscriberTwice_ShouldNotDuplicate()
    {
        var subscriber = new Mock<INotificationSubscriber>();

        EventBus.Instance.Subscribe(subscriber.Object);
        EventBus.Instance.Subscribe(subscriber.Object); // second subscribe — no-op

        var notification = new Notification { Message = "Duplicate test" };
        EventBus.Instance.Publish(notification);

        // Should have been called only once despite subscribing twice
        subscriber.Verify(s => s.Receive(notification), Times.Once);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Unsubscribe
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>Unsubscribed subscriber should not receive further notifications.</summary>
    [Fact]
    public void Unsubscribe_Subscriber_ShouldNotReceiveFurtherNotifications()
    {
        var subscriber = new Mock<INotificationSubscriber>();

        EventBus.Instance.Subscribe(subscriber.Object);
        EventBus.Instance.Unsubscribe(subscriber.Object);

        EventBus.Instance.Publish(new Notification { Message = "After unsubscribe" });

        subscriber.Verify(s => s.Receive(It.IsAny<Notification>()), Times.Never);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Null guards
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>Subscribe with null subscriber should throw ArgumentNullException.</summary>
    [Fact]
    public void Subscribe_NullSubscriber_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => EventBus.Instance.Subscribe(null!));
    }

    /// <summary>Publish with null notification should throw ArgumentNullException.</summary>
    [Fact]
    public void Publish_NullNotification_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => EventBus.Instance.Publish(null!));
    }
}
