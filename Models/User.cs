namespace SmartHomeAutomationSystem.Models;

/// <summary>
/// Abstract base class representing a system user with role-based identification.
/// Implements the Template Method pattern — subclasses define <see cref="Role"/>.
/// </summary>
public abstract class User
{
    /// <summary>Gets or sets the unique identifier for this user.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the display name of the user.</summary>
    public required string Name { get; set; }

    /// <summary>Gets the role of the user (e.g. "Admin", "Homeowner").</summary>
    public abstract string Role { get; }

    /// <summary>Returns a formatted string representation of the user.</summary>
    public override string ToString()
    {
        return $"ID: {Id}, Name: {Name}, Role: {Role}";
    }
}

/// <summary>Represents an administrative user with full system access.</summary>
public class AdminUser : User
{
    /// <inheritdoc/>
    public override string Role => "Admin";
}

/// <summary>Represents a homeowner user who manages their own smart home devices.</summary>
public class HomeownerUser : User
{
    /// <inheritdoc/>
    public override string Role => "Homeowner";
}
