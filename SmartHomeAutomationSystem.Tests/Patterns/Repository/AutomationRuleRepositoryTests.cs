using SmartHomeAutomationSystem.Models;
using SmartHomeAutomationSystem.Patterns.Repository;
using Xunit;

namespace SmartHomeAutomationSystem.Tests.Patterns.Repository;

/// <summary>
/// Unit tests for <see cref="AutomationRuleRepository"/>.
/// </summary>
public class AutomationRuleRepositoryTests
{
    private AutomationRuleRepository CreateRepository() => new();

    /// <summary>Add followed by GetAll should contain the new rule.</summary>
    [Fact]
    public void Add_ShouldMakeRuleRetrievableViaGetAll()
    {
        var repo = CreateRepository();
        var rule = new AutomationRule { Name = "Test", Condition = "cond", Action = "act" };

        repo.Add(rule);

        Assert.Single(repo.GetAll());
    }

    /// <summary>GetById should return the rule when it exists.</summary>
    [Fact]
    public void GetById_WhenRuleExists_ShouldReturnRule()
    {
        var repo = CreateRepository();
        var rule = new AutomationRule { Name = "Night Off", Condition = "time", Action = "turn off" };
        repo.Add(rule);

        var result = repo.GetById(rule.Id);

        Assert.NotNull(result);
        Assert.Equal("Night Off", result.Name);
    }

    /// <summary>Update should change rule fields and return true.</summary>
    [Fact]
    public void Update_WhenRuleExists_ShouldUpdateFields()
    {
        var repo = CreateRepository();
        var rule = new AutomationRule { Name = "Old", Condition = "c", Action = "a" };
        repo.Add(rule);

        var updated = new AutomationRule { Name = "New", Condition = "c2", Action = "a2" };
        var result = repo.Update(rule.Id, updated);

        Assert.True(result);
        Assert.Equal("New", repo.GetById(rule.Id)!.Name);
    }

    /// <summary>Delete should remove the rule and return true.</summary>
    [Fact]
    public void Delete_WhenRuleExists_ShouldRemoveAndReturnTrue()
    {
        var repo = CreateRepository();
        var rule = new AutomationRule { Name = "R", Condition = "c", Action = "a" };
        repo.Add(rule);

        var result = repo.Delete(rule.Id);

        Assert.True(result);
        Assert.Empty(repo.GetAll());
    }

    /// <summary>Delete should return false when rule does not exist.</summary>
    [Fact]
    public void Delete_WhenRuleNotFound_ShouldReturnFalse()
    {
        var repo = CreateRepository();

        var result = repo.Delete(Guid.NewGuid());

        Assert.False(result);
    }
}
