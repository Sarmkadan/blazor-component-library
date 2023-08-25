// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorComponentLibrary.Models;

/// <summary>
/// Configuration for Modal dialog components.
/// Defines appearance, behavior, and button actions.
/// </summary>
public class ModalConfig
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("modalType")]
    public ModalType ModalType { get; set; } = ModalType.Default;

    [JsonPropertyName("size")]
    public ModalSize Size { get; set; } = ModalSize.Medium;

    [JsonPropertyName("isPrimary")]
    public bool IsPrimary { get; set; } = false;

    [JsonPropertyName("isClosable")]
    public bool IsClosable { get; set; } = true;

    [JsonPropertyName("isBackdropClickable")]
    public bool IsBackdropClickable { get; set; } = true;

    [JsonPropertyName("showHeader")]
    public bool ShowHeader { get; set; } = true;

    [JsonPropertyName("showFooter")]
    public bool ShowFooter { get; set; } = true;

    [JsonPropertyName("primaryButtonText")]
    public string PrimaryButtonText { get; set; } = "Confirm";

    [JsonPropertyName("secondaryButtonText")]
    public string SecondaryButtonText { get; set; } = "Cancel";

    [JsonPropertyName("primaryButtonClass")]
    public string? PrimaryButtonClass { get; set; }

    [JsonPropertyName("secondaryButtonClass")]
    public string? SecondaryButtonClass { get; set; }

    [JsonPropertyName("headerClass")]
    public string? HeaderClass { get; set; }

    [JsonPropertyName("bodyClass")]
    public string? BodyClass { get; set; }

    [JsonPropertyName("footerClass")]
    public string? FooterClass { get; set; }

    [JsonPropertyName("maxWidth")]
    public string? MaxWidth { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the CSS class for the modal based on its type.
    /// </summary>
    public string GetModalTypeClass()
    {
        return ModalType switch
        {
            ModalType.Success => "modal-success",
            ModalType.Error => "modal-error",
            ModalType.Warning => "modal-warning",
            ModalType.Info => "modal-info",
            _ => "modal-default"
        };
    }

    /// <summary>
    /// Gets the CSS class for the modal size.
    /// </summary>
    public string GetSizeClass()
    {
        return Size switch
        {
            ModalSize.Small => "modal-sm",
            ModalSize.Large => "modal-lg",
            ModalSize.ExtraLarge => "modal-xl",
            _ => "modal-md"
        };
    }

    /// <summary>
    /// Validates the modal configuration.
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Title) &&
               !string.IsNullOrWhiteSpace(PrimaryButtonText) &&
               !string.IsNullOrWhiteSpace(SecondaryButtonText) &&
               Title.Length >= 1 &&
               Title.Length <= 100;
    }

    /// <summary>
    /// Creates a copy of this modal configuration.
    /// </summary>
    public ModalConfig Clone()
    {
        return new ModalConfig
        {
            Id = Id,
            Title = Title,
            Content = Content,
            ModalType = ModalType,
            Size = Size,
            IsPrimary = IsPrimary,
            IsClosable = IsClosable,
            IsBackdropClickable = IsBackdropClickable,
            ShowHeader = ShowHeader,
            ShowFooter = ShowFooter,
            PrimaryButtonText = PrimaryButtonText,
            SecondaryButtonText = SecondaryButtonText,
            PrimaryButtonClass = PrimaryButtonClass,
            SecondaryButtonClass = SecondaryButtonClass,
            HeaderClass = HeaderClass,
            BodyClass = BodyClass,
            FooterClass = FooterClass,
            MaxWidth = MaxWidth,
            CreatedAt = CreatedAt
        };
    }

    /// <summary>
    /// Sets CSS classes for all modal parts at once.
    /// </summary>
    public void SetAllClasses(string? headerClass, string? bodyClass, string? footerClass)
    {
        HeaderClass = headerClass;
        BodyClass = bodyClass;
        FooterClass = footerClass;
    }
}

public enum ModalType
{
    Default = 0,
    Success = 1,
    Error = 2,
    Warning = 3,
    Info = 4
}

public enum ModalSize
{
    Small = 0,
    Medium = 1,
    Large = 2,
    ExtraLarge = 3
}
