// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;
using BlazorComponentLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorComponentLibrary.Controllers;

/// <summary>
/// REST API controller for component management.
/// Provides endpoints for CRUD operations, searching, and bulk updates.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ComponentController : ControllerBase
{
    private readonly ComponentService _componentService;
    private readonly ILogger<ComponentController> _logger;

    public ComponentController(ComponentService componentService, ILogger<ComponentController> logger)
    {
        _componentService = componentService ?? throw new ArgumentNullException(nameof(componentService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves all active components with optional type filtering.
    /// Supports pagination and search parameters.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ComponentConfig>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllComponents([FromQuery] string? type = null)
    {
        try
        {
            _logger.LogInformation("Fetching components with type filter: {ComponentType}", type ?? "none");
            var components = await _componentService.GetAllComponentsAsync(type);
            return Ok(components);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching components");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Retrieves a specific component by ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ComponentConfig), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetComponentById(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("Component ID must be greater than 0");

            _logger.LogInformation("Fetching component with ID: {ComponentId}", id);
            var component = await _componentService.GetComponentByIdAsync(id);

            if (component == null)
                return NotFound($"Component with ID {id} not found");

            return Ok(component);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching component {ComponentId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Creates a new component configuration.
    /// Validates input and applies default values before persistence.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ComponentConfig), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateComponent([FromBody] ComponentConfig config)
    {
        try
        {
            if (config == null)
                return BadRequest("Component configuration cannot be null");

            if (!config.IsValid())
                return BadRequest("Component configuration validation failed");

            _logger.LogInformation("Creating component: {ComponentName}", config.Name);
            var created = await _componentService.CreateComponentAsync(config);
            return CreatedAtAction(nameof(GetComponentById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid component configuration");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating component");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Updates an existing component configuration.
    /// Preserves creation date and updates modification timestamp.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ComponentConfig), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateComponent(int id, [FromBody] ComponentConfig config)
    {
        try
        {
            if (id <= 0)
                return BadRequest("Component ID must be greater than 0");

            if (config == null)
                return BadRequest("Component configuration cannot be null");

            if (!config.IsValid())
                return BadRequest("Component configuration validation failed");

            _logger.LogInformation("Updating component: {ComponentId}", id);
            var updated = await _componentService.UpdateComponentAsync(id, config);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Component not found: {ComponentId}", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating component {ComponentId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Soft deletes a component by marking it as inactive.
    /// Physical deletion is not supported to maintain referential integrity.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteComponent(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("Component ID must be greater than 0");

            _logger.LogInformation("Deleting component: {ComponentId}", id);
            var deleted = await _componentService.DeleteComponentAsync(id);

            if (!deleted)
                return NotFound($"Component with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting component {ComponentId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Searches components by name or description.
    /// Returns only active components matching the search term.
    /// </summary>
    [HttpGet("search/{searchTerm}")]
    [ProducesResponseType(typeof(IEnumerable<ComponentConfig>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchComponents(string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Search term cannot be empty");

            _logger.LogInformation("Searching components with term: {SearchTerm}", searchTerm);
            var results = await _componentService.SearchComponentsAsync(searchTerm);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching components");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Retrieves usage statistics across all components.
    /// Useful for monitoring and analytics dashboards.
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ComponentStatistics), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatistics()
    {
        try
        {
            _logger.LogInformation("Fetching component statistics");
            var stats = await _componentService.GetComponentStatisticsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching statistics");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Updates display order for multiple components in bulk.
    /// Useful for drag-and-drop reordering operations.
    /// </summary>
    [HttpPost("reorder")]
    [ProducesResponseType(typeof(IEnumerable<ComponentConfig>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReorderComponents([FromBody] List<(int id, int order)> updates)
    {
        try
        {
            if (updates == null || updates.Count == 0)
                return BadRequest("Updates list cannot be empty");

            _logger.LogInformation("Reordering {ComponentCount} components", updates.Count);
            var result = await _componentService.UpdateComponentOrderAsync(updates);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering components");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }
}
