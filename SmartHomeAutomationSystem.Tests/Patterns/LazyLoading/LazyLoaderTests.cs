using SmartHomeAutomationSystem.Patterns.LazyLoading;
using Xunit;

namespace SmartHomeAutomationSystem.Tests.Patterns.LazyLoading;

/// <summary>
/// Unit tests for <see cref="LazyLoader{T}"/> (Lazy Initialization pattern).
/// </summary>
public class LazyLoaderTests
{
    // ──────────────────────────────────────────────────────────────────────────
    // Value
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>Value should return the instance created by the factory.</summary>
    [Fact]
    public void Value_ShouldReturnFactoryResult()
    {
        var loader = new LazyLoader<string>(() => "hello");

        Assert.Equal("hello", loader.Value);
    }

    /// <summary>Factory should only be invoked once even when Value is accessed multiple times.</summary>
    [Fact]
    public void Value_AccessedMultipleTimes_ShouldCallFactoryOnlyOnce()
    {
        int callCount = 0;
        var loader = new LazyLoader<string>(() => { callCount++; return "value"; });

        _ = loader.Value;
        _ = loader.Value;
        _ = loader.Value;

        Assert.Equal(1, callCount);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // IsValueCreated
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>IsValueCreated should be false before the first access.</summary>
    [Fact]
    public void IsValueCreated_BeforeAccess_ShouldBeFalse()
    {
        var loader = new LazyLoader<object>(() => new object());

        Assert.False(loader.IsValueCreated);
    }

    /// <summary>IsValueCreated should be true after Value is accessed.</summary>
    [Fact]
    public void IsValueCreated_AfterAccess_ShouldBeTrue()
    {
        var loader = new LazyLoader<object>(() => new object());
        _ = loader.Value;

        Assert.True(loader.IsValueCreated);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Constructor guard
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>Passing null factory should throw ArgumentNullException.</summary>
    [Fact]
    public void Constructor_NullFactory_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new LazyLoader<object>(null!));
    }
}
