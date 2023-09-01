// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace BlazorComponentLibrary.Models;

/// <summary>
/// Represents the configuration metadata for a reusable Blazor component.
/// Stores component display settings, behavior flags, and styling preferences.
/// </summary>
public class ComponentConfig
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [JsonProperty("componentType")]
    public string ComponentType { get; set; } = string.Empty;

    [JsonProperty("isActive")]
    public bool IsActive { get; set; } = true;

    [JsonProperty("version")]
    public string Version { get; set; } = "1.0.0";

    [JsonProperty("cssClass")]
    public string? CssClass { get; set; }

    [JsonProperty("attributes")]
    public Dictionary<string, string>? Attributes { get; set; } = new();

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonProperty("modifiedAt")]
    public DateTime? ModifiedAt { get; set; }

    [Range(0, 1000)]
    [JsonProperty("displayOrder")]
    public int DisplayOrder { get; set; } = 0;

    /// <summary>
    /// Validates the component configuration for required properties.
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Name) &&
               !string.IsNullOrWhiteSpace(ComponentType) &&
               !string.IsNullOrWhiteSpace(Description) &&
               Name.Length >= 3 &&
               Name.Length <= 100 &&
               Description.Length <= 500;
    }

    /// <summary>
    /// Merges attribute dictionary with provided overrides.
    /// </summary>
    public Dictionary<string, string> GetMergedAttributes(Dictionary<string, string>? overrides = null)
    {
        var merged = new Dictionary<string, string>(Attributes ?? new());
        if (overrides != null)
        {
            foreach (var kvp in overrides)
            {
                merged[kvp.Key] = kvp.Value;
            }
        }
        return merged;
    }

    /// <summary>
    /// Creates a clone of this configuration with updated modification time.
    /// </summary>
    public ComponentConfig Clone()
    {
        return new ComponentConfig
        {
            Id = Id,
            Name = Name,
            Description = Description,
            ComponentType = ComponentType,
            IsActive = IsActive,
            Version = Version,
            CssClass = CssClass,
            Attributes = Attributes != null ? new Dictionary<string, string>(Attributes) : null,
            CreatedAt = CreatedAt,
            ModifiedAt = DateTime.UtcNow,
            DisplayOrder = DisplayOrder
        };
    }
}
