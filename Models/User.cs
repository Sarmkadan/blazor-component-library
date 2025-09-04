// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace BlazorComponentLibrary.Models;

/// <summary>
/// Represents a user with authentication and preference management.
/// Supports role-based access control and customization settings.
/// </summary>
public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    [JsonProperty("username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(254)]
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    [JsonProperty("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [StringLength(100)]
    [JsonProperty("firstName")]
    public string? FirstName { get; set; }

    [StringLength(100)]
    [JsonProperty("lastName")]
    public string? LastName { get; set; }

    [JsonProperty("role")]
    public UserRole Role { get; set; } = UserRole.User;

    [JsonProperty("isActive")]
    public bool IsActive { get; set; } = true;

    [JsonProperty("preferences")]
    public UserPreferences Preferences { get; set; } = new();

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonProperty("lastLogin")]
    public DateTime? LastLogin { get; set; }

    [JsonProperty("lastModified")]
    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the user's full name combining first and last names.
    /// </summary>
    public string GetFullName()
    {
        if (string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName))
        {
            return Username;
        }

        var parts = new[] { FirstName, LastName }
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();

        return string.Join(" ", parts);
    }

    /// <summary>
    /// Validates that the user has required properties.
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Username) &&
               !string.IsNullOrWhiteSpace(Email) &&
               !string.IsNullOrWhiteSpace(PasswordHash) &&
               Username.Length >= 3 &&
               Username.Length <= 100 &&
               Email.Length <= 254;
    }

    /// <summary>
    /// Checks if the user has a specific permission.
    /// </summary>
    public bool HasPermission(string permission)
    {
        return Role switch
        {
            UserRole.Admin => true,
            UserRole.Moderator => !permission.StartsWith("admin."),
            UserRole.User => permission.StartsWith("user."),
            _ => false
        };
    }

    /// <summary>
    /// Updates the last login timestamp to now.
    /// </summary>
    public void UpdateLastLogin()
    {
        LastLogin = DateTime.UtcNow;
        LastModified = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a user summary without sensitive information.
    /// </summary>
    public UserSummary ToSummary()
    {
        return new UserSummary
        {
            Id = Id,
            Username = Username,
            Email = Email,
            FullName = GetFullName(),
            Role = Role,
            IsActive = IsActive
        };
    }

    /// <summary>
    /// Updates user profile information.
    /// </summary>
    public void UpdateProfile(string? firstName, string? lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        LastModified = DateTime.UtcNow;
    }
}

public class UserPreferences
{
    [JsonProperty("themeId")]
    public int? ThemeId { get; set; }

    [JsonProperty("locale")]
    public string Locale { get; set; } = "en-US";

    [JsonProperty("itemsPerPage")]
    public int ItemsPerPage { get; set; } = 25;

    [JsonProperty("enableNotifications")]
    public bool EnableNotifications { get; set; } = true;

    [JsonProperty("enableEmailNotifications")]
    public bool EnableEmailNotifications { get; set; } = true;

    [JsonProperty("timezone")]
    public string Timezone { get; set; } = "UTC";

    [JsonProperty("defaultView")]
    public string DefaultView { get; set; } = "grid";
}

public class UserSummary
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; } = string.Empty;

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("fullName")]
    public string FullName { get; set; } = string.Empty;

    [JsonProperty("role")]
    public UserRole Role { get; set; }

    [JsonProperty("isActive")]
    public bool IsActive { get; set; }
}

public enum UserRole
{
    User = 0,
    Moderator = 1,
    Admin = 2
}
