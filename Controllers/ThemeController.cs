// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;
using BlazorComponentLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorComponentLibrary.Controllers;

/// <summary>
/// REST API controller for theme management.
/// Handles theme configuration, application, and customization.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ThemeController : ControllerBase
{
    private readonly ThemeService _themeService;
    private readonly ILogger<ThemeController> _logger;

    public ThemeController(ThemeService themeService, ILogger<ThemeController> logger)
    {
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves all available themes.
    /// Includes both built-in and custom user themes.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Theme>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllThemes()
    {
        try
        {
            _logger.LogInformation("Fetching all available themes");
            var themes = await _themeService.GetAllThemesAsync();
            return Ok(themes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching themes");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Retrieves a specific theme by ID.
    /// Includes all color definitions and configuration.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Theme), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetThemeById(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("Theme ID must be greater than 0");

            _logger.LogInformation("Fetching theme with ID: {ThemeId}", id);
            var theme = await _themeService.GetThemeByIdAsync(id);

            if (theme == null)
                return NotFound($"Theme with ID {id} not found");

            return Ok(theme);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching theme {ThemeId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Creates a new custom theme.
    /// Validates color values and applies defaults for missing properties.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Theme), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTheme([FromBody] CreateThemeRequest request)
    {
        try
        {
            if (request == null)
                return BadRequest("Theme request cannot be null");

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Theme name is required");

            _logger.LogInformation("Creating new theme: {ThemeName}", request.Name);
            var theme = await _themeService.CreateThemeAsync(request);
            return CreatedAtAction(nameof(GetThemeById), new { id = theme.Id }, theme);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid theme configuration");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating theme");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Updates an existing theme.
    /// Preserves theme creation date and updates modification timestamp.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Theme), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTheme(int id, [FromBody] CreateThemeRequest request)
    {
        try
        {
            if (id <= 0)
                return BadRequest("Theme ID must be greater than 0");

            if (request == null)
                return BadRequest("Theme request cannot be null");

            _logger.LogInformation("Updating theme: {ThemeId}", id);
            var updated = await _themeService.UpdateThemeAsync(id, request);

            if (updated == null)
                return NotFound($"Theme with ID {id} not found");

            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating theme {ThemeId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Deletes a theme by ID.
    /// Built-in themes cannot be deleted.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteTheme(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("Theme ID must be greater than 0");

            _logger.LogInformation("Deleting theme: {ThemeId}", id);
            var deleted = await _themeService.DeleteThemeAsync(id);

            if (!deleted)
                return NotFound($"Theme with ID {id} not found");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot delete built-in theme");
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting theme {ThemeId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Generates CSS output from theme definition.
    /// Useful for previewing and exporting theme styles.
    /// </summary>
    [HttpGet("{id}/css")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExportThemeAsCSS(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("Theme ID must be greater than 0");

            _logger.LogInformation("Exporting theme {ThemeId} as CSS", id);
            var css = await _themeService.GenerateThemeCSSAsync(id);

            if (string.IsNullOrEmpty(css))
                return NotFound($"Theme with ID {id} not found");

            return Ok(new { css });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting theme as CSS");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Duplicates an existing theme with a new name.
    /// Useful for creating variations from established themes.
    /// </summary>
    [HttpPost("{id}/duplicate")]
    [ProducesResponseType(typeof(Theme), StatusCodes.Status201Created)]
    public async Task<IActionResult> DuplicateTheme(int id, [FromBody] DuplicateThemeRequest request)
    {
        try
        {
            if (id <= 0)
                return BadRequest("Theme ID must be greater than 0");

            if (request == null || string.IsNullOrWhiteSpace(request.NewName))
                return BadRequest("New theme name is required");

            _logger.LogInformation("Duplicating theme {ThemeId} as {NewName}", id, request.NewName);
            var duplicate = await _themeService.DuplicateThemeAsync(id, request.NewName);

            if (duplicate == null)
                return NotFound($"Theme with ID {id} not found");

            return CreatedAtAction(nameof(GetThemeById), new { id = duplicate.Id }, duplicate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error duplicating theme");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Validates theme colors and configuration.
    /// Returns validation errors without persisting.
    /// </summary>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ThemeValidationResult), StatusCodes.Status200OK)]
    public IActionResult ValidateTheme([FromBody] CreateThemeRequest request)
    {
        try
        {
            if (request == null)
                return BadRequest("Theme request cannot be null");

            _logger.LogInformation("Validating theme configuration");
            var result = _themeService.ValidateTheme(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating theme");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }
}

public class CreateThemeRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Dictionary<string, string> Colors { get; set; } = new();
    public Dictionary<string, string>? Variables { get; set; }
    public bool IsDefault { get; set; }
}

public class DuplicateThemeRequest
{
    public string NewName { get; set; } = string.Empty;
}

public class ThemeValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}
