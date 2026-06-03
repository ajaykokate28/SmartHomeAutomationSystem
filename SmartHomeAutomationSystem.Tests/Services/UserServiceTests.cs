using Moq;
using SmartHomeAutomationSystem.Models;
using SmartHomeAutomationSystem.Patterns.Repository;
using SmartHomeAutomationSystem.Services;
using Xunit;

namespace SmartHomeAutomationSystem.Tests.Services;

/// <summary>
/// Unit tests for <see cref="UserService"/>.
/// </summary>
public class UserServiceTests
{
    private readonly Mock<IRepository<User>> _repositoryMock;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _repositoryMock = new Mock<IRepository<User>>();
        _sut = new UserService(_repositoryMock.Object);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // RegisterUser
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>RegisterUser with role "Admin" should add an AdminUser to the repository.</summary>
    [Fact]
    public void RegisterUser_WithAdminRole_ShouldAddAdminUserToRepository()
    {
        _sut.RegisterUser("Ajay", "Admin");

        _repositoryMock.Verify(
            r => r.Add(It.Is<User>(u => u is AdminUser && u.Name == "Ajay")),
            Times.Once);
    }

    /// <summary>RegisterUser with role "Homeowner" should add a HomeownerUser.</summary>
    [Fact]
    public void RegisterUser_WithHomeownerRole_ShouldAddHomeownerUserToRepository()
    {
        _sut.RegisterUser("Vijay", "Homeowner");

        _repositoryMock.Verify(
            r => r.Add(It.Is<User>(u => u is HomeownerUser && u.Name == "Vijay")),
            Times.Once);
    }

    /// <summary>RegisterUser with an unknown role should not call repository.Add.</summary>
    [Fact]
    public void RegisterUser_WithInvalidRole_ShouldNotCallRepositoryAdd()
    {
        _sut.RegisterUser("Unknown", "Superuser");

        _repositoryMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // UpdateUser
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>UpdateUser should update the name when the user exists.</summary>
    [Fact]
    public void UpdateUser_WhenUserExists_ShouldCallRepositoryUpdate()
    {
        var id = Guid.NewGuid();
        var existingUser = new AdminUser { Id = id, Name = "OldName" };
        _repositoryMock.Setup(r => r.GetById(id)).Returns(existingUser);
        _repositoryMock.Setup(r => r.Update(id, It.IsAny<User>())).Returns(true);

        _sut.UpdateUser(id, "NewName");

        _repositoryMock.Verify(r => r.Update(id, It.Is<User>(u => u.Name == "NewName")), Times.Once);
    }

    /// <summary>UpdateUser should not call repository.Update when user is not found.</summary>
    [Fact]
    public void UpdateUser_WhenUserNotFound_ShouldNotCallRepositoryUpdate()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetById(id)).Returns((User?)null);

        _sut.UpdateUser(id, "NewName");

        _repositoryMock.Verify(r => r.Update(It.IsAny<Guid>(), It.IsAny<User>()), Times.Never);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // DeleteUser
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>DeleteUser should call repository.Delete with the correct id.</summary>
    [Fact]
    public void DeleteUser_WhenUserExists_ShouldCallRepositoryDelete()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.Delete(id)).Returns(true);

        _sut.DeleteUser(id);

        _repositoryMock.Verify(r => r.Delete(id), Times.Once);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // GetAllUsers
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>GetAllUsers should return the complete list from the repository.</summary>
    [Fact]
    public void GetAllUsers_ShouldReturnAllUsersFromRepository()
    {
        var users = new List<User>
        {
            new AdminUser { Name = "Ajay" },
            new HomeownerUser { Name = "Vijay" }
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(users);

        var result = _sut.GetAllUsers();

        Assert.Equal(2, result.Count);
    }
}
