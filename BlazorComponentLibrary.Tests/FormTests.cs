using BlazorComponentLibrary.Components.Form;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace BlazorComponentLibrary.Tests;

public class FormTests
{
    private class TestModel
    {
        public string? Name { get; set; }
    }

    private class ValidatedModel
    {
        [Required]
        public string? Name { get; set; }

        [Range(1, 120)]
        public int Age { get; set; } = 30;
    }

    [Fact]
    public void SetModel_ShouldUpdateModel()
    {
        // Arrange
        var form = new Form<TestModel>();
        var newModel = new TestModel { Name = "Test" };

        // Act
        form.SetModel(newModel);

        // Assert
        form.Model.Should().Be(newModel);
    }

    [Fact]
    public void DefaultModel_ShouldNotBeNull()
    {
        // Arrange
        var form = new Form<TestModel>();

        // Assert
        form.Model.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Validate_ModelWithoutAttributes_ReturnsTrue()
    {
        // Arrange
        var form = new Form<TestModel>();

        // Act
        var result = await form.Validate();

        // Assert
        result.Should().BeTrue();
        form.IsValid.Should().BeTrue();
        form.ValidationErrors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_InvalidModel_ReturnsFalseAndExposesErrors()
    {
        // Arrange
        var form = new Form<ValidatedModel>();
        form.SetModel(new ValidatedModel { Name = null, Age = 0 });

        // Act
        var result = await form.Validate();

        // Assert
        result.Should().BeFalse();
        form.IsValid.Should().BeFalse();
        form.ValidationErrors.Should().HaveCount(2);
    }

    [Fact]
    public async Task Validate_ValidModel_ReturnsTrue()
    {
        // Arrange
        var form = new Form<ValidatedModel>();
        form.SetModel(new ValidatedModel { Name = "Test", Age = 30 });

        // Act
        var result = await form.Validate();

        // Assert
        result.Should().BeTrue();
        form.ValidationErrors.Should().BeEmpty();
    }

    [Fact]
    public async Task SetModel_AfterFailedValidation_ResetsValidationState()
    {
        // Arrange
        var form = new Form<ValidatedModel>();
        form.SetModel(new ValidatedModel { Name = null });
        await form.Validate();

        // Act
        form.SetModel(new ValidatedModel { Name = "Test" });

        // Assert
        form.IsValid.Should().BeTrue();
        form.ValidationErrors.Should().BeEmpty();
    }
}
