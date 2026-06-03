namespace SmartHomeAutomationSystem.Patterns.ResourcePool;

/// <summary>
/// Thread-safe generic Object Pool — reuses expensive objects to reduce allocation overhead.
/// Demonstrates the Object Pool pattern.
/// </summary>
public class ResourcePool<T>
{
    private readonly Stack<T> _pool = new();
    private readonly Func<T> _factory;
    private readonly int _maxSize;
    private readonly object _lock = new();

    /// <summary>Initialises a new pool with the specified factory and maximum capacity.</summary>
    /// <param name="factory">A delegate used to create new resources when the pool is empty.</param>
    /// <param name="maxSize">Maximum number of idle resources retained in the pool (default: 10).</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="factory"/> is <c>null</c>.</exception>
    public ResourcePool(Func<T> factory, int maxSize = 10)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _factory = factory;
        _maxSize = maxSize;
    }

    /// <summary>Acquires a resource from the pool, creating one if the pool is empty.</summary>
    public T Acquire()
    {
        lock (_lock)
        {
            return _pool.Count > 0 ? _pool.Pop() : _factory();
        }
    }

    /// <summary>Returns a resource back to the pool for reuse.</summary>
    public void Release(T resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        lock (_lock)
        {
            if (_pool.Count < _maxSize)
                _pool.Push(resource);
        }
    }

    /// <summary>Number of resources currently available in the pool.</summary>
    public int AvailableCount
    {
        get { lock (_lock) { return _pool.Count; } }
    }
}
