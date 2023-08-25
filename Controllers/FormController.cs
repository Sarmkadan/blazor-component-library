// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;
using BlazorComponentLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorComponentLibrary.Controllers;

/// <summary>
/// REST API controller for form management and submissions.
/// Handles form configuration, validation, and submission processing.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class FormController : ControllerBase
{
    private readonly FormService _formService;
    private readonly ILogger<FormController> _logger;

    public FormController(FormService formService, ILogger<FormController> logger)
    {
        _formService = formService ?? throw new ArgumentNullException(nameof(formService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves all available form templates.
    /// Forms are categorized by purpose and reusability.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FormField>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllForms([FromQuery] string? category = null)
    {
        try
        {
            _logger.LogInformation("Fetching forms with category filter: {Category}", category ?? "all");
            var forms = await _formService.GetAllFormsAsync(category);
            return Ok(forms);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching forms");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Retrieves a specific form configuration by ID.
    /// Includes all field definitions and validation rules.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(FormField), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFormById(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("Form ID must be greater than 0");

            _logger.LogInformation("Fetching form with ID: {FormId}", id);
            var form = await _formService.GetFormByIdAsync(id);

            if (form == null)
                return NotFound($"Form with ID {id} not found");

            return Ok(form);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching form {FormId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Validates form submission data against defined rules.
    /// Returns validation errors without persisting data.
    /// </summary>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(FormValidationResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateSubmission([FromBody] FormSubmission submission)
    {
        try
        {
            if (submission == null)
                return BadRequest("Form submission cannot be null");

            _logger.LogInformation("Validating form submission");
            var result = await _formService.ValidateSubmissionAsync(submission);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating form submission");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Processes a form submission with validation and persistence.
    /// Applies business logic and triggers post-submit workflows.
    /// </summary>
    [HttpPost("submit")]
    [ProducesResponseType(typeof(FormSubmissionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitForm([FromBody] FormSubmission submission)
    {
        try
        {
            if (submission == null)
                return BadRequest("Form submission cannot be null");

            _logger.LogInformation("Processing form submission");
            var response = await _formService.ProcessSubmissionAsync(submission);

            if (!response.IsSuccessful)
                return BadRequest(response);

            return Ok(response);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Form validation failed");
            return BadRequest(new FormSubmissionResponse { Errors = new[] { ex.Message } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing form submission");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Creates a new form template with field definitions.
    /// Used by administrators to build custom forms.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(FormField), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateForm([FromBody] CreateFormRequest request)
    {
        try
        {
            if (request == null)
                return BadRequest("Form request cannot be null");

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Form name is required");

            _logger.LogInformation("Creating new form: {FormName}", request.Name);
            var form = await _formService.CreateFormAsync(request);
            return CreatedAtAction(nameof(GetFormById), new { id = form.Id }, form);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid form configuration");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating form");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Updates an existing form template.
    /// Changes only affect new submissions, previous submissions are archived.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(FormField), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateForm(int id, [FromBody] CreateFormRequest request)
    {
        try
        {
            if (id <= 0)
                return BadRequest("Form ID must be greater than 0");

            if (request == null)
                return BadRequest("Form request cannot be null");

            _logger.LogInformation("Updating form: {FormId}", id);
            var updated = await _formService.UpdateFormAsync(id, request);

            if (updated == null)
                return NotFound($"Form with ID {id} not found");

            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating form {FormId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Retrieves all submissions for a specific form.
    /// Supports filtering and pagination for large result sets.
    /// </summary>
    [HttpGet("{formId}/submissions")]
    [ProducesResponseType(typeof(FormSubmissionPage), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFormSubmissions(int formId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            if (formId <= 0)
                return BadRequest("Form ID must be greater than 0");

            if (page < 1 || pageSize < 1)
                return BadRequest("Page and PageSize must be greater than 0");

            _logger.LogInformation("Fetching submissions for form {FormId}: Page {Page}", formId, page);
            var submissions = await _formService.GetFormSubmissionsAsync(formId, page, pageSize);
            return Ok(submissions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching form submissions");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Exports form submissions in specified format.
    /// Useful for reports and data analysis.
    /// </summary>
    [HttpGet("{formId}/export")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportSubmissions(int formId, [FromQuery] string format = "CSV")
    {
        try
        {
            if (formId <= 0)
                return BadRequest("Form ID must be greater than 0");

            _logger.LogInformation("Exporting submissions for form {FormId} in format {Format}", formId, format);
            var fileContent = await _formService.ExportSubmissionsAsync(formId, format);
            var fileName = $"submissions_{formId}_{DateTime.UtcNow:yyyyMMdd}.{format.ToLower()}";
            return File(fileContent, GetMimeType(format), fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting submissions");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    private string GetMimeType(string format) => format.ToUpper() switch
    {
        "CSV" => "text/csv",
        "JSON" => "application/json",
        "XML" => "application/xml",
        _ => "application/octet-stream"
    };
}

public class CreateFormRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = "General";
    public List<FormField> Fields { get; set; } = new();
}

public class FormSubmission
{
    public int FormId { get; set; }
    public Dictionary<string, object> FieldValues { get; set; } = new();
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}

public class FormValidationResult
{
    public bool IsValid { get; set; }
    public Dictionary<string, string> FieldErrors { get; set; } = new();
}

public class FormSubmissionResponse
{
    public bool IsSuccessful { get; set; }
    public int? SubmissionId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string[]? Errors { get; set; }
}

public class FormSubmissionPage
{
    public List<FormSubmission> Submissions { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
