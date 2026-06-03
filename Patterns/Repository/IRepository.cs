namespace SmartHomeAutomationSystem.Patterns.Repository;

/// <summary>
/// Generic repository interface following the Repository pattern.
/// Satisfies ISP and DIP — consumers depend on abstraction, not implementation.
/// </summary>
/// <typeparam name="T">The entity type managed by this repository.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>Adds a new entity to the repository.</summary>
    void Add(T entity);

    /// <summary>Updates an existing entity identified by <paramref name="id"/>.</summary>
    /// <returns><c>true</c> if found and updated; otherwise <c>false</c>.</returns>
    bool Update(Guid id, T updatedEntity);

    /// <summary>Removes the entity with the specified <paramref name="id"/>.</summary>
    /// <returns><c>true</c> if found and removed; otherwise <c>false</c>.</returns>
    bool Delete(Guid id);

    /// <summary>Returns all entities in the repository.</summary>
    List<T> GetAll();

    /// <summary>Returns the entity with the specified <paramref name="id"/>, or <c>null</c>.</summary>
    T? GetById(Guid id);
}
