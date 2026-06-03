using SmartHomeAutomationSystem.Models;

namespace SmartHomeAutomationSystem.Services.Interfaces;

/// <summary>
/// Defines registration, update, deletion, and query operations for system users.
/// </summary>
public interface IUserService
{
    /// <summary>Registers a new user with the given name and role.</summary>
    /// <param name="name">The user's display name.</param>
    /// <param name="role">The user's role (e.g. Admin, Homeowner).</param>
    void RegisterUser(string name, string role);

    /// <summary>Updates the name of an existing user.</summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="newName">The new display name.</param>
    void UpdateUser(Guid id, string newName);

    /// <summary>Deletes the user with the specified <paramref name="id"/>.</summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    void DeleteUser(Guid id);

    /// <summary>Writes all users to the console.</summary>
    void ViewUsers();

    /// <summary>Returns all registered users.</summary>
    List<User> GetAllUsers();
}
