using SmartHomeAutomationSystem.Models;

namespace SmartHomeAutomationSystem.Patterns.Factory
{
    /// <summary>
    /// Factory class for creating role-specific <see cref="User"/> instances.
    /// Implements the Factory Method pattern — centralises object creation and
    /// isolates the caller from concrete user subtypes.
    /// </summary>
    public static class UserFactory
    {
        /// <summary>
        /// Creates a <see cref="User"/> instance for the specified role.
        /// </summary>
        /// <param name="role">The role string (case-insensitive): "admin" or "homeowner".</param>
        /// <param name="name">The display name for the new user.</param>
        /// <returns>A concrete <see cref="User"/> subtype matching the role.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="role"/> is not recognised.</exception>
        public static User CreateUser(string role, string name)
        {
            return role.ToLower() switch
            {
                "admin" => new AdminUser { Name = name },
                "homeowner" => new HomeownerUser { Name = name },
                _ => throw new ArgumentException("Invalid role specified.")
            };
        }
    }
}
