// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Utilities;
using FluentAssertions;

namespace BlazorComponentLibrary.Tests;

public class StringHelperTests
{
    [Fact]
    public void ToKebabCase_PascalCaseInput_ReturnsDashSeparatedLowercase()
    {
        // Arrange
        const string input = "DataTableComponent";

        // Act
        var result = StringHelper.ToKebabCase(input);

        // Assert
        result.Should().Be("data-table-component");
    }

    [Fact]
    public void ToSnakeCase_MultiWordPascalCase_ReturnsUnderscoreSeparatedLowercase()
    {
        // Arrange
        const string input = "FormFieldName";

        // Act
        var result = StringHelper.ToSnakeCase(input);

        // Assert
        result.Should().Be("form_field_name");
    }

    [Fact]
    public void Truncate_StringLongerThanMaxLength_AppendsDefaultEllipsis()
    {
        // Arrange
        const string input = "Hello, World!";

        // Act
        var result = StringHelper.Truncate(input, 8);

        // Assert
        result.Should().Be("Hello...");
        result.Should().HaveLength(8);
    }

    [Fact]
    public void Truncate_StringShorterThanMaxLength_ReturnsOriginalString()
    {
        // Arrange
        const string input = "Hi";

        // Act
        var result = StringHelper.Truncate(input, 10);

        // Assert
        result.Should().Be("Hi");
    }

    [Fact]
    public void IsValidEmail_WellFormedAddress_ReturnsTrue()
    {
        // Act
        var result = StringHelper.IsValidEmail("admin@example.com");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CountOccurrences_RepeatedSubstringInSource_ReturnsCorrectCount()
    {
        // Arrange
        const string source = "the cat sat on the mat";

        // Act
        var result = StringHelper.CountOccurrences(source, "the");

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public void ToUrlSlug_StringWithSpacesAndSpecialChars_ReturnsDashSeparatedSlug()
    {
        // Arrange
        const string input = "My Component Title!";

        // Act
        var result = StringHelper.ToUrlSlug(input);

        // Assert
        result.Should().Be("my-component-title");
        result.Should().NotContain(" ");
        result.Should().NotContain("!");
    }
}
