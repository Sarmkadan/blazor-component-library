// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Security.Cryptography;
using System.Text;
using BlazorComponentLibrary.Models;
using BlazorComponentLibrary.Repositories;

namespace BlazorComponentLibrary.Services;

/// <summary>
/// Service for user management, authentication, and preferences.
/// Handles user CRUD, password hashing, and role-based access control.
/// </summary>
public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    /// Creates a new user with hashed password.
    /// </summary>
    public async Task<UserSummary> CreateUserAsync(string username, string email, string password, string? firstName = null, string? lastName = null)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required", nameof(username));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required", nameof(password));

        if (password.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters", nameof(password));

        var existingUser = await GetUserByUsernameAsync(username);
        if (existingUser != null)
            throw new InvalidOperationException($"Username '{username}' already exists");

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = HashPassword(password),
            FirstName = firstName,
            LastName = lastName,
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        if (!user.IsValid())
            throw new InvalidOperationException("User data is invalid");

        var created = await _userRepository.CreateAsync(user);
        return created.ToSummary();
    }

    /// <summary>
    /// Gets a user by ID.
    /// </summary>
    public async Task<User?> GetUserByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("ID must be greater than 0", nameof(id));

        return await _userRepository.GetByIdAsync(id);
    }

    /// <summary>
    /// Gets a user by username.
    /// </summary>
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));

        var users = await _userRepository.GetAllAsync();
        return users.FirstOrDefault(u => u.Username == username);
    }

    /// <summary>
    /// Gets a user by email address.
    /// </summary>
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        var users = await _userRepository.GetAllAsync();
        return users.FirstOrDefault(u => u.Email == email);
    }

    /// <summary>
    /// Authenticates a user with username and password.
    /// </summary>
    public async Task<UserSummary?> AuthenticateAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return null;

        var user = await GetUserByUsernameAsync(username);
        if (user == null || !user.IsActive)
            return null;

        if (!VerifyPassword(password, user.PasswordHash))
            return null;

        user.UpdateLastLogin();
        await _userRepository.UpdateAsync(user);

        return user.ToSummary();
    }

    /// <summary>
    /// Updates user profile information.
    /// </summary>
    public async Task<UserSummary> UpdateProfileAsync(int id, string? firstName, string? lastName)
    {
        if (id <= 0)
            throw new ArgumentException("ID must be greater than 0", nameof(id));

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found");

        user.UpdateProfile(firstName, lastName);
        var updated = await _userRepository.UpdateAsync(user);
        return updated.ToSummary();
    }

    /// <summary>
    /// Changes a user's password.
    /// </summary>
    public async Task<bool> ChangePasswordAsync(int id, string oldPassword, string newPassword)
    {
        if (id <= 0)
            throw new ArgumentException("ID must be greater than 0", nameof(id));

        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("New password is required", nameof(newPassword));

        if (newPassword.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters", nameof(newPassword));

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found");

        if (!VerifyPassword(oldPassword, user.PasswordHash))
            throw new InvalidOperationException("Current password is incorrect");

        user.PasswordHash = HashPassword(newPassword);
        user.LastModified = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        return true;
    }

    /// <summary>
    /// Updates user preferences.
    /// </summary>
    public async Task<User> UpdatePreferencesAsync(int id, UserPreferences preferences)
    {
        if (id <= 0)
            throw new ArgumentException("ID must be greater than 0", nameof(id));

        if (preferences == null)
            throw new ArgumentNullException(nameof(preferences));

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found");

        user.Preferences = preferences;
        user.LastModified = DateTime.UtcNow;

        return await _userRepository.UpdateAsync(user);
    }

    /// <summary>
    /// Updates a user's role.
    /// </summary>
    public async Task<UserSummary> UpdateRoleAsync(int id, UserRole role)
    {
        if (id <= 0)
            throw new ArgumentException("ID must be greater than 0", nameof(id));

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found");

        user.Role = role;
        user.LastModified = DateTime.UtcNow;
        var updated = await _userRepository.UpdateAsync(user);
        return updated.ToSummary();
    }

    /// <summary>
    /// Activates or deactivates a user account.
    /// </summary>
    public async Task<bool> SetUserActiveStatusAsync(int id, bool isActive)
    {
        if (id <= 0)
            throw new ArgumentException("ID must be greater than 0", nameof(id));

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found");

        user.IsActive = isActive;
        user.LastModified = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        return true;
    }

    /// <summary>
    /// Gets all active users.
    /// </summary>
    public async Task<IEnumerable<UserSummary>> GetActiveUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Where(u => u.IsActive).Select(u => u.ToSummary());
    }

    /// <summary>
    /// Gets users by role.
    /// </summary>
    public async Task<IEnumerable<UserSummary>> GetUsersByRoleAsync(UserRole role)
    {
        var users = await _userRepository.GetAllAsync();
        return users.Where(u => u.Role == role).Select(u => u.ToSummary());
    }

    /// <summary>
    /// Searches users by username or email.
    /// </summary>
    public async Task<IEnumerable<UserSummary>> SearchUsersAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Enumerable.Empty<UserSummary>();

        var users = await _userRepository.GetAllAsync();
        var term = searchTerm.ToLower();

        return users.Where(u =>
            u.Username.ToLower().Contains(term) ||
            u.Email.ToLower().Contains(term) ||
            (u.FirstName?.ToLower().Contains(term) ?? false) ||
            (u.LastName?.ToLower().Contains(term) ?? false)
        ).Select(u => u.ToSummary());
    }

    /// <summary>
    /// Gets user statistics.
    /// </summary>
    public async Task<UserStatistics> GetUserStatisticsAsync()
    {
        var users = (await _userRepository.GetAllAsync()).ToList();

        return new UserStatistics
        {
            TotalUsers = users.Count,
            ActiveUsers = users.Count(u => u.IsActive),
            InactiveUsers = users.Count(u => !u.IsActive),
            AdminCount = users.Count(u => u.Role == UserRole.Admin),
            ModeratorCount = users.Count(u => u.Role == UserRole.Moderator),
            RegularUserCount = users.Count(u => u.Role == UserRole.User),
            LastUpdated = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Hashes a password using SHA256.
    /// </summary>
    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    /// <summary>
    /// Verifies a password against its hash.
    /// </summary>
    private bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == hash;
    }
}

public class UserStatistics
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int AdminCount { get; set; }
    public int ModeratorCount { get; set; }
    public int RegularUserCount { get; set; }
    public DateTime LastUpdated { get; set; }
}
