namespace SmartHomeAutomationSystem.Patterns.Interceptor;

/// <summary>
/// Abstract base for the Interceptor pattern — wraps an operation with
/// pre/post processing (e.g. logging, validation, auditing).
/// Subclasses override BeforeExecute / AfterExecute to add cross-cutting concerns.
/// </summary>
public abstract class InterceptorBase
{
    /// <summary>Called before the main operation executes.</summary>
    protected virtual void BeforeExecute(string operationName)
    {
        Console.WriteLine($"[Interceptor] Before: {operationName}");
    }

    /// <summary>Called after the main operation executes.</summary>
    protected virtual void AfterExecute(string operationName)
    {
        Console.WriteLine($"[Interceptor] After: {operationName}");
    }

    /// <summary>Wraps an action with before/after interception hooks.</summary>
    public void Execute(string operationName, Action operation)
    {
        BeforeExecute(operationName);
        operation();
        AfterExecute(operationName);
    }
}

/// <summary>
/// Concrete interceptor that logs execution time for operations.
/// </summary>
public class LoggingInterceptor : InterceptorBase
{
    private System.Diagnostics.Stopwatch? _stopwatch;

    /// <inheritdoc/>
    protected override void BeforeExecute(string operationName)
    {
        _stopwatch = System.Diagnostics.Stopwatch.StartNew();
        Console.WriteLine($"[LoggingInterceptor] Starting '{operationName}'");
    }

    /// <inheritdoc/>
    protected override void AfterExecute(string operationName)
    {
        _stopwatch?.Stop();
        Console.WriteLine($"[LoggingInterceptor] Completed '{operationName}' in {_stopwatch?.ElapsedMilliseconds}ms");
    }
}
