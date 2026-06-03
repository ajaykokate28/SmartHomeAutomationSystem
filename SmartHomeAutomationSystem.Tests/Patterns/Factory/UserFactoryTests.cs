using SmartHomeAutomationSystem.Models;
using SmartHomeAutomationSystem.Patterns.Factory;
using Xunit;

namespace SmartHomeAutomationSystem.Tests.Patterns.Factory;

/// <summary>
/// Unit tests for <see cref="UserFactory"/>.
/// </summary>
public class UserFactoryTests
{
    // ──────────────────────────────────────────────────────────────────────────
    // CreateUser — valid roles
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>CreateUser with "Admin" role should return an AdminUser with the correct name.</summary>
    [Fact]
    public void CreateUser_WithAdminRole_ShouldReturnAdminUser()
    {
        var user = UserFactory.CreateUser("Admin", "Ajay");

        Assert.IsType<AdminUser>(user);
        Assert.Equal("Ajay", user.Name);
        Assert.Equal("Admin", user.Role);
    }

    /// <summary>CreateUser with "Homeowner" role should return a HomeownerUser.</summary>
    [Fact]
    public void CreateUser_WithHomeownerRole_ShouldReturnHomeownerUser()
    {
        var user = UserFactory.CreateUser("Homeowner", "Vijay");

        Assert.IsType<HomeownerUser>(user);
        Assert.Equal("Vijay", user.Name);
        Assert.Equal("Homeowner", user.Role);
    }

    /// <summary>CreateUser is case-insensitive for the role parameter.</summary>
    [Theory]
    [InlineData("admin")]
    [InlineData("ADMIN")]
    [InlineData("Admin")]
    public void CreateUser_AdminRole_IsCaseInsensitive(string role)
    {
        var user = UserFactory.CreateUser(role, "Test");

        Assert.IsType<AdminUser>(user);
    }

    /// <summary>CreateUser is case-insensitive for homeowner role.</summary>
    [Theory]
    [InlineData("homeowner")]
    [InlineData("HOMEOWNER")]
    [InlineData("Homeowner")]
    public void CreateUser_HomeownerRole_IsCaseInsensitive(string role)
    {
        var user = UserFactory.CreateUser(role, "Test");

        Assert.IsType<HomeownerUser>(user);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // CreateUser — invalid role
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>CreateUser with an unknown role should throw ArgumentException.</summary>
    [Fact]
    public void CreateUser_WithInvalidRole_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => UserFactory.CreateUser("superuser", "Test"));
    }

    /// <summary>CreateUser with empty role should throw ArgumentException.</summary>
    [Fact]
    public void CreateUser_WithEmptyRole_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => UserFactory.CreateUser(string.Empty, "Test"));
    }
}
