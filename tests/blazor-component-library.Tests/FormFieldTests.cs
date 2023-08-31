// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;
using FluentAssertions;

namespace BlazorComponentLibrary.Tests;

public class FormFieldTests
{
    [Fact]
    public void Validate_RequiredFieldWithNullValue_ReturnsValidationError()
    {
        // Arrange
        var field = new FormField
        {
            Name = "username",
            Label = "Username",
            IsRequired = true
        };

        // Act
        var result = field.Validate(null);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("required");
    }

    [Fact]
    public void Validate_ValueExceedsMaxLength_ReturnsMaxLengthError()
    {
        // Arrange
        var field = new FormField
        {
            Name = "shortfield",
            Label = "Short Field",
            MaxLength = 5
        };

        // Act
        var result = field.Validate("toolongvalue");

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("must not exceed");
    }

    [Fact]
    public void Validate_EmailFieldWithoutAtSign_ReturnsEmailFormatError()
    {
        // Arrange
        var field = new FormField
        {
            Name = "email",
            Label = "Email",
            FieldType = FormFieldType.Email
        };

        // Act
        var result = field.Validate("notavalidemail");

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("valid email");
    }

    [Fact]
    public void Validate_ValidValueWithinLengthConstraints_ReturnsSuccess()
    {
        // Arrange
        var field = new FormField
        {
            Name = "description",
            Label = "Description",
            MinLength = 3,
            MaxLength = 20
        };

        // Act
        var result = field.Validate("Hello");

        // Assert
        result.IsValid.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void Copy_FieldWithOptions_CreatesCopyWithMatchingValues()
    {
        // Arrange
        var field = new FormField
        {
            Id = 7,
            Name = "status",
            Label = "Status",
            FieldType = FormFieldType.Select,
            IsRequired = true,
            Order = 3,
            Options = new List<FormFieldOption>
            {
                new() { Value = "active", Label = "Active" },
                new() { Value = "inactive", Label = "Inactive" }
            }
        };

        // Act
        var copy = field.Copy();

        // Assert
        copy.Name.Should().Be(field.Name);
        copy.Label.Should().Be(field.Label);
        copy.IsRequired.Should().Be(field.IsRequired);
        copy.Options.Should().NotBeNull();
        copy.Options.Should().HaveCount(2);
        copy.Options.Should().NotBeSameAs(field.Options);
    }
}
