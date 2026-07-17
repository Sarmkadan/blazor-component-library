using BlazorComponentLibrary.Components.Form;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using Xunit;

/// <summary>
/// Tests for the Form component.
/// </summary>
public class FormTests
{
    /// <summary>
    /// A test model for testing the Form component.
    /// </summary>
    private class TestModel
    {
        public string? Name { get; set; }
    }

    /// <summary>
    /// A validated model for testing the Form component.
    /// </summary>
    private class ValidatedModel
    {
        /// <summary>
        /// Gets or sets the name of the model.
        /// </summary>
        /// <value>The name of the model.</value>
        [Required]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the age of the model.
        /// </summary>
        /// <value>The age of the model.</value>
        [Range(1, 120)]
        public int Age { get; set; } = 30;
    }

    /// <summary>
    /// Tests that setting a new model updates the model.
    /// </summary>
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

    /// <summary>
    /// Tests that the default model is not null.
    /// </summary>
    [Fact]
    public void DefaultModel_ShouldNotBeNull()
    {
        // Arrange
        var form = new Form<TestModel>();

        // Assert
        form.Model.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that a model without validation attributes returns true.
    /// </summary>
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

    /// <summary>
    /// Tests that an invalid model returns false and exposes errors.
    /// </summary>
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

    /// <summary>
    /// Tests that a valid model returns true.
    /// </summary>
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

    /// <summary>
    /// Tests that setting a new model after failed validation resets the validation state.
    /// </summary>
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
