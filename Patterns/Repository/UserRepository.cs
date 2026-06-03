using SmartHomeAutomationSystem.Models;

namespace SmartHomeAutomationSystem.Patterns.Repository
{
    /// <summary>In-memory <see cref="IRepository{T}"/> implementation for <see cref="User"/> entities.</summary>
    public class UserRepository : IRepository<User>
    {
        private readonly List<User> _users = new();

        /// <inheritdoc/>
        public void Add(User user) => _users.Add(user);

        /// <inheritdoc/>
        public bool Update(Guid id, User updatedUser)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null) return false;

            user.Name = updatedUser.Name;
            return true;
        }

        /// <inheritdoc/>
        public bool Delete(Guid id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            return user != null && _users.Remove(user);
        }

        /// <inheritdoc/>
        public List<User> GetAll() => _users;

        /// <inheritdoc/>
        public User? GetById(Guid id) => _users.FirstOrDefault(u => u.Id == id);
    }
}
