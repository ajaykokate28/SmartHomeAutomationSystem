using SmartHomeAutomationSystem.Models;
using SmartHomeAutomationSystem.Patterns.Repository;
using Xunit;

namespace SmartHomeAutomationSystem.Tests.Patterns.Repository;

/// <summary>
/// Unit tests for <see cref="UserRepository"/>.
/// </summary>
public class UserRepositoryTests
{
    private UserRepository CreateRepository() => new();

    /// <summary>Add followed by GetAll should contain the added user.</summary>
    [Fact]
    public void Add_ShouldMakeUserRetrievableViaGetAll()
    {
        var repo = CreateRepository();
        var user = new AdminUser { Name = "Ajay" };

        repo.Add(user);

        Assert.Single(repo.GetAll());
    }

    /// <summary>GetById should return the user when it exists.</summary>
    [Fact]
    public void GetById_WhenUserExists_ShouldReturnUser()
    {
        var repo = CreateRepository();
        var user = new AdminUser { Name = "Ajay" };
        repo.Add(user);

        var result = repo.GetById(user.Id);

        Assert.NotNull(result);
        Assert.Equal("Ajay", result.Name);
    }

    /// <summary>Update should change the user's name and return true.</summary>
    [Fact]
    public void Update_WhenUserExists_ShouldUpdateName()
    {
        var repo = CreateRepository();
        var user = new AdminUser { Name = "OldName" };
        repo.Add(user);

        var updated = new AdminUser { Id = user.Id, Name = "NewName" };
        var result = repo.Update(user.Id, updated);

        Assert.True(result);
        Assert.Equal("NewName", repo.GetById(user.Id)!.Name);
    }

    /// <summary>Update should return false when user does not exist.</summary>
    [Fact]
    public void Update_WhenUserNotFound_ShouldReturnFalse()
    {
        var repo = CreateRepository();
        var updated = new AdminUser { Name = "Ghost" };

        var result = repo.Update(Guid.NewGuid(), updated);

        Assert.False(result);
    }

    /// <summary>Delete should remove the user and return true.</summary>
    [Fact]
    public void Delete_WhenUserExists_ShouldRemoveAndReturnTrue()
    {
        var repo = CreateRepository();
        var user = new HomeownerUser { Name = "Vijay" };
        repo.Add(user);

        var result = repo.Delete(user.Id);

        Assert.True(result);
        Assert.Empty(repo.GetAll());
    }

    /// <summary>Delete should return false when user does not exist.</summary>
    [Fact]
    public void Delete_WhenUserNotFound_ShouldReturnFalse()
    {
        var repo = CreateRepository();

        var result = repo.Delete(Guid.NewGuid());

        Assert.False(result);
    }
}
