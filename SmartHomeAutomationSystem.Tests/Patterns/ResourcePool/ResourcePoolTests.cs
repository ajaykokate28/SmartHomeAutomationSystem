using SmartHomeAutomationSystem.Patterns.ResourcePool;
using System.Text;
using Xunit;

namespace SmartHomeAutomationSystem.Tests.Patterns.ResourcePool;

/// <summary>
/// Unit tests for <see cref="ResourcePool{T}"/> (Object Pool pattern).
/// </summary>
public class ResourcePoolTests
{
    // ──────────────────────────────────────────────────────────────────────────
    // Acquire
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>Acquire on an empty pool should create a new instance via the factory.</summary>
    [Fact]
    public void Acquire_WhenPoolIsEmpty_ShouldCreateNewInstance()
    {
        int factoryCallCount = 0;
        var pool = new ResourcePool<string>(() => { factoryCallCount++; return "resource"; });

        var resource = pool.Acquire();

        Assert.Equal("resource", resource);
        Assert.Equal(1, factoryCallCount);
    }

    /// <summary>Acquire after Release should reuse the previously released resource.</summary>
    [Fact]
    public void Acquire_AfterRelease_ShouldReuseResource()
    {
        int factoryCallCount = 0;
        var pool = new ResourcePool<string>(() => { factoryCallCount++; return $"resource-{factoryCallCount}"; });

        var first = pool.Acquire();
        pool.Release(first);
        var second = pool.Acquire();

        Assert.Same(first, second);
        Assert.Equal(1, factoryCallCount); // factory called only once
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Release
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>Release should increase AvailableCount by one.</summary>
    [Fact]
    public void Release_ShouldIncreaseAvailableCount()
    {
        var pool = new ResourcePool<StringBuilder>(() => new StringBuilder(), maxSize: 5);

        pool.Release(new StringBuilder());

        Assert.Equal(1, pool.AvailableCount);
    }

    /// <summary>Release beyond maxSize should not exceed the cap.</summary>
    [Fact]
    public void Release_BeyondMaxSize_ShouldNotExceedMaxSize()
    {
        var pool = new ResourcePool<object>(() => new object(), maxSize: 2);

        pool.Release(new object());
        pool.Release(new object());
        pool.Release(new object()); // overflow — discarded

        Assert.Equal(2, pool.AvailableCount);
    }

    /// <summary>Release with null should throw ArgumentNullException.</summary>
    [Fact]
    public void Release_NullResource_ShouldThrowArgumentNullException()
    {
        var pool = new ResourcePool<object>(() => new object());

        Assert.Throws<ArgumentNullException>(() => pool.Release(null!));
    }

    // ──────────────────────────────────────────────────────────────────────────
    // AvailableCount
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>AvailableCount should be zero for a freshly created pool.</summary>
    [Fact]
    public void AvailableCount_NewPool_ShouldBeZero()
    {
        var pool = new ResourcePool<object>(() => new object());

        Assert.Equal(0, pool.AvailableCount);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Constructor guard
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>Passing null factory to ResourcePool should throw ArgumentNullException.</summary>
    [Fact]
    public void Constructor_NullFactory_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ResourcePool<object>(null!));
    }
}
