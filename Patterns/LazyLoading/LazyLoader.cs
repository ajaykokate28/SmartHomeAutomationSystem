namespace SmartHomeAutomationSystem.Patterns.LazyLoading;

/// <summary>
/// Generic Lazy Loading wrapper — defers expensive object creation until first access.
/// Demonstrates the Lazy Initialization pattern.
/// </summary>
public class LazyLoader<T>
{
    private readonly Lazy<T> _lazy;

    /// <summary>Initialises a new <see cref="LazyLoader{T}"/> with the specified factory.</summary>
    /// <param name="factory">A delegate that creates the value on first access.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="factory"/> is <c>null</c>.</exception>
    public LazyLoader(Func<T> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _lazy = new Lazy<T>(factory, isThreadSafe: true);
    }

    /// <summary>Gets the lazily-initialized value.</summary>
    public T Value => _lazy.Value;

    /// <summary>Indicates whether the value has been created yet.</summary>
    public bool IsValueCreated => _lazy.IsValueCreated;
}
