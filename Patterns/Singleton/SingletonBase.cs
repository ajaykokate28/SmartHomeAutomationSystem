namespace SmartHomeAutomationSystem.Patterns.Singleton;

/// <summary>
/// Thread-safe generic Singleton base class using <see cref="Lazy{T}"/>.
/// Subclasses must have a public parameterless constructor.
/// </summary>
/// <typeparam name="T">The type to be instantiated as a singleton.</typeparam>
public abstract class SingletonBase<T> where T : class, new()
{
    private static readonly Lazy<T> _instance = new(() => new T());

    /// <summary>Gets the single shared instance of <typeparamref name="T"/>.</summary>
    public static T Instance => _instance.Value;
}