using SmartHomeAutomationSystem.Models;
using SmartHomeAutomationSystem.Patterns.Factory;
using SmartHomeAutomationSystem.Patterns.Repository;
using SmartHomeAutomationSystem.Services.Interfaces;

namespace SmartHomeAutomationSystem.Services;

/// <summary>
/// Manages user registration, update, deletion, and queries.
/// Uses <see cref="UserFactory"/> (Factory pattern) to instantiate role-specific user types.
/// </summary>
public class UserService : IUserService
{
    private readonly IRepository<User> _repository;

    /// <summary>Initialises a new instance with the specified user repository.</summary>
    /// <param name="repository">The user repository to use for persistence.</param>
    public UserService(IRepository<User> repository)
    {
        _repository = repository;
    }

    /// <inheritdoc/>
    public void RegisterUser(string name, string role)
    {
        try
        {
            var user = UserFactory.CreateUser(role, name);
            _repository.Add(user);
            Console.WriteLine("User registered successfully.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error registering user: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public void UpdateUser(Guid id, string newName)
    {
        var existing = _repository.GetById(id);
        if (existing == null)
        {
            Console.WriteLine("User not found.");
            return;
        }

        existing.Name = newName;
        if (_repository.Update(id, existing))
            Console.WriteLine("User updated.");
        else
            Console.WriteLine("User not found.");
    }

    /// <inheritdoc/>
    public void DeleteUser(Guid id)
    {
        if (_repository.Delete(id))
            Console.WriteLine("User deleted.");
        else
            Console.WriteLine("User not found.");
    }

    /// <inheritdoc/>
    public void ViewUsers()
    {
        var users = _repository.GetAll();
        if (users.Count == 0)
        {
            Console.WriteLine("No users found.");
            return;
        }

        foreach (var user in users)
            Console.WriteLine(user);
    }

    /// <inheritdoc/>
    public List<User> GetAllUsers() => _repository.GetAll();
}

