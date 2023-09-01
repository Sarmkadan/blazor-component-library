// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

namespace BlazorComponentLibrary.Constants;

/// <summary>
/// Global constants used throughout the component library.
/// </summary>
public static class ApplicationConstants
{
    public const string LibraryName = "Blazor Component Library";
    public const string LibraryVersion = "1.0.0";
    public const string LibraryAuthor = "Vladyslav Zaiets";
    public const string LibraryWebsite = "https://sarmkadan.com";

    public static class Defaults
    {
        public const int PageSize = 25;
        public const int MaxPageSize = 1000;
        public const int DefaultThemeId = 1;
        public const string DefaultLocale = "en-US";
        public const string DefaultTimezone = "UTC";
        public const int DefaultItemsPerPage = 25;
    }

    public static class Validation
    {
        public const int MinimumPasswordLength = 6;
        public const int MaximumUsernameLength = 100;
        public const int MinimumUsernameLength = 3;
        public const int MaximumEmailLength = 254;
        public const int MaximumComponentNameLength = 100;
        public const int MinimumComponentNameLength = 3;
        public const int MaximumDescriptionLength = 500;
    }

    public static class FieldSizes
    {
        public const int MinColumnWidth = 10;
        public const int MaxColumnWidth = 500;
        public const int DefaultColumnWidth = 100;
        public const int MinBaseFontSize = 8;
        public const int MaxBaseFontSize = 32;
        public const double MinLineHeight = 1.0;
        public const double MaxLineHeight = 4.0;
    }

    public static class ErrorMessages
    {
        public const string InvalidConfiguration = "Configuration is invalid";
        public const string NotFound = "Resource not found";
        public const string Unauthorized = "Unauthorized access";
        public const string Forbidden = "Access forbidden";
        public const string Conflict = "Resource already exists";
        public const string ValidationFailed = "Validation failed";
    }

    public static class CacheKeys
    {
        public const string ComponentPrefix = "component_";
        public const string ThemePrefix = "theme_";
        public const string UserPrefix = "user_";
        public const string ComponentListKey = "components_list";
        public const string ThemeListKey = "themes_list";
        public const string UserListKey = "users_list";
    }
}

/// <summary>
/// Permission constants for role-based access control.
/// </summary>
public static class Permissions
{
    public static class Admin
    {
        public const string ManageUsers = "admin.manage_users";
        public const string ManageRoles = "admin.manage_roles";
        public const string ManageThemes = "admin.manage_themes";
        public const string ViewLogs = "admin.view_logs";
    }

    public static class User
    {
        public const string ViewProfile = "user.view_profile";
        public const string EditProfile = "user.edit_profile";
        public const string ViewComponents = "user.view_components";
        public const string ManagePreferences = "user.manage_preferences";
    }

    public static class Moderator
    {
        public const string ManageComponents = "moderator.manage_components";
        public const string ManageContent = "moderator.manage_content";
        public const string ViewReports = "moderator.view_reports";
    }
}

/// <summary>
/// HTTP status code constants.
/// </summary>
public static class HttpStatusCodes
{
    public const int OK = 200;
    public const int Created = 201;
    public const int BadRequest = 400;
    public const int Unauthorized = 401;
    public const int Forbidden = 403;
    public const int NotFound = 404;
    public const int Conflict = 409;
    public const int InternalServerError = 500;
    public const int ServiceUnavailable = 503;
}
