// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;

namespace BlazorComponentLibrary.Repositories;

/// <summary>
/// In-memory implementation of the user repository.
/// Manages user accounts and authentication data.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    private int _nextId = 1;

    public async Task<User> CreateAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        user.Id = _nextId++;
        user.CreatedAt = DateTime.UtcNow;
        user.LastModified = DateTime.UtcNow;
        _users.Add(user);

        return await Task.FromResult(user);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await Task.FromResult(_users.AsEnumerable());
    }

    public async Task<User> UpdateAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var existing = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existing == null)
            throw new KeyNotFoundException($"User with ID {user.Id} not found");

        var index = _users.IndexOf(existing);
        user.LastModified = DateTime.UtcNow;
        _users[index] = user;

        return await Task.FromResult(user);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return await Task.FromResult(false);

        _users.Remove(user);
        return await Task.FromResult(true);
    }
}
