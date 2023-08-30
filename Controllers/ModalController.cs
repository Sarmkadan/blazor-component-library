// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlazorComponentLibrary.Controllers;

/// <summary>
/// REST API controller for modal window management.
/// Handles modal configuration, validation, and state management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ModalController : ControllerBase
{
    private readonly ILogger<ModalController> _logger;
    private static readonly Dictionary<string, ModalConfig> _modals = new();

    public ModalController(ILogger<ModalController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves all registered modal configurations.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ModalConfig>), StatusCodes.Status200OK)]
    public IActionResult GetAllModals()
    {
        try
        {
            _logger.LogInformation("Fetching all modals");
            return Ok(_modals.Values);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching modals");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Retrieves specific modal by ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ModalConfig), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetModalById(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Modal ID cannot be empty");

            _logger.LogInformation("Fetching modal: {ModalId}", id);

            if (_modals.TryGetValue(id, out var modal))
                return Ok(modal);

            return NotFound($"Modal with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching modal {ModalId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Creates a new modal configuration.
    /// Validates configuration before persistence.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ModalConfig), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult CreateModal([FromBody] ModalConfig config)
    {
        try
        {
            if (config == null)
                return BadRequest("Modal configuration cannot be null");

            if (string.IsNullOrEmpty(config.Title))
                return BadRequest("Modal title is required");

            var id = Guid.NewGuid().ToString();
            config.Id = id;
            config.CreatedAt = DateTime.UtcNow;

            _logger.LogInformation("Creating modal: {ModalId}", id);
            _modals[id] = config;

            return CreatedAtAction(nameof(GetModalById), new { id }, config);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating modal");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Updates existing modal configuration.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ModalConfig), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateModal(string id, [FromBody] ModalConfig config)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Modal ID cannot be empty");

            if (config == null)
                return BadRequest("Modal configuration cannot be null");

            if (!_modals.ContainsKey(id))
                return NotFound($"Modal with ID {id} not found");

            _logger.LogInformation("Updating modal: {ModalId}", id);
            config.Id = id;
            config.ModifiedAt = DateTime.UtcNow;
            _modals[id] = config;

            return Ok(config);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating modal {ModalId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Deletes modal configuration by ID.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteModal(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Modal ID cannot be empty");

            if (!_modals.ContainsKey(id))
                return NotFound($"Modal with ID {id} not found");

            _logger.LogInformation("Deleting modal: {ModalId}", id);
            _modals.Remove(id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting modal {ModalId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Shows modal by bringing it to front and setting visible.
    /// </summary>
    [HttpPost("{id}/show")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult ShowModal(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Modal ID cannot be empty");

            if (!_modals.TryGetValue(id, out var modal))
                return NotFound($"Modal with ID {id} not found");

            modal.IsVisible = true;
            modal.ZIndex = 1000;
            _logger.LogInformation("Modal shown: {ModalId}", id);

            return Ok(new { message = "Modal shown", modal });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing modal {ModalId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Hides modal by setting visible to false.
    /// </summary>
    [HttpPost("{id}/hide")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult HideModal(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Modal ID cannot be empty");

            if (!_modals.TryGetValue(id, out var modal))
                return NotFound($"Modal with ID {id} not found");

            modal.IsVisible = false;
            _logger.LogInformation("Modal hidden: {ModalId}", id);

            return Ok(new { message = "Modal hidden", modal });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error hiding modal {ModalId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Gets modal state (visible/hidden).
    /// </summary>
    [HttpGet("{id}/state")]
    [ProducesResponseType(typeof(ModalStateResponse), StatusCodes.Status200OK)]
    public IActionResult GetModalState(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Modal ID cannot be empty");

            if (!_modals.TryGetValue(id, out var modal))
                return NotFound($"Modal with ID {id} not found");

            return Ok(new ModalStateResponse
            {
                Id = id,
                IsVisible = modal.IsVisible,
                Title = modal.Title,
                ZIndex = modal.ZIndex
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting modal state {ModalId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }
}

/// <summary>
/// Response model for modal state queries.
/// </summary>
public class ModalStateResponse
{
    public string Id { get; set; } = string.Empty;
    public bool IsVisible { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ZIndex { get; set; }
}
