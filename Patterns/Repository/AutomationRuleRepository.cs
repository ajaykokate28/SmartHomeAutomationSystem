using SmartHomeAutomationSystem.Models;

namespace SmartHomeAutomationSystem.Patterns.Repository
{
    /// <summary>In-memory <see cref="IRepository{T}"/> implementation for <see cref="AutomationRule"/> entities.</summary>
    public class AutomationRuleRepository : IRepository<AutomationRule>
    {
        private readonly List<AutomationRule> _rules = new();

        /// <inheritdoc/>
        public void Add(AutomationRule rule) => _rules.Add(rule);

        /// <inheritdoc/>
        public bool Update(Guid id, AutomationRule updatedRule)
        {
            var rule = _rules.FirstOrDefault(r => r.Id == id);
            if (rule == null) return false;

            rule.Name = updatedRule.Name;
            rule.Condition = updatedRule.Condition;
            rule.Action = updatedRule.Action;
            rule.Schedule = updatedRule.Schedule;
            return true;
        }

        /// <inheritdoc/>
        public bool Delete(Guid id)
        {
            var rule = _rules.FirstOrDefault(r => r.Id == id);
            return rule != null && _rules.Remove(rule);
        }

        /// <inheritdoc/>
        public List<AutomationRule> GetAll() => _rules;

        /// <inheritdoc/>
        public AutomationRule? GetById(Guid id) => _rules.FirstOrDefault(r => r.Id == id);
    }
}
