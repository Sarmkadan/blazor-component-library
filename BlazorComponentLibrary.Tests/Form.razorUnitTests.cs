namespace BlazorComponentLibrary.Tests;

using Bunit;
using Xunit;
using BlazorComponentLibrary.Components.Form;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

/// <summary>
/// Comprehensive unit tests for the <see cref="Form{TModel}"/> component public API.
/// Tests cover happy-path scenarios, edge cases (null/empty inputs, boundary values),
/// and error-path assertions.
/// </summary>
public sealed class FormRazorUnitTests : TestContext
{
    /// <summary>
    /// A simple test model without validation attributes.
    /// </summary>
    private class SimpleModel
    {
        public string? Name { get; set; }
        public int Count { get; set; }
    }

    /// <summary>
    /// A validated model with required and range attributes.
    /// </summary>
    private class ValidatedModel : IValidatableObject
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name must be less than 100 characters")]
        public string? Name { get; set; }

        [Range(0, 1000, ErrorMessage = "Count must be between 0 and 1000")]
        public int Count { get; set; }

        [Range(18, 120, ErrorMessage = "Age must be between 18 and 120")]
        public int Age { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name != null && Name.Length > 50)
            {
                yield return new ValidationResult("Name must be less than 50 characters", [nameof(Name)]);
            }
        }
    }

    /// <summary>
    /// A model with complex validation.
    /// </summary>
    private class ComplexModel
    {
        [Required]
        public string? RequiredField { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }

    /// <summary>
    /// Verifies that the component initializes with default values when no parameters are provided.
    /// </summary>
    [Fact]
    public void DefaultRender_HasDefaultValues()
    {
        // Arrange & Act
        var cut = RenderComponent<Form<SimpleModel>>();

        // Assert
        cut.Instance.Model.Should().NotBeNull();
        cut.Instance.IsValid.Should().BeTrue();
        cut.Instance.ValidationErrors.Should().BeEmpty();
        cut.Instance.IsDirty.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that the form renders with child content.
    /// </summary>
    [Fact]
    public void Render_WithChildContent_DisplaysContent()
    {
        // Arrange & Act
        var cut = RenderComponent<Form<SimpleModel>>(parameters => parameters
            .AddChildContent("<input type=\"text\" />"));

        // Assert
        cut.Markup.Should().Contain("<input type=\"text\" />");
        cut.Markup.Should().Contain("<form class=\"bcl-form\"");
    }

    /// <summary>
    /// Verifies that setting a model via SetModel updates the internal model.
    /// </summary>
    [Fact]
    public void SetModel_UpdatesInternalModel()
    {
        // Arrange
        var cut = RenderComponent<Form<SimpleModel>>();
        var newModel = new SimpleModel { Name = "Test Name", Count = 42 };

        // Act
        cut.Instance.SetModel(newModel);

        // Assert
        cut.Instance.Model.Should().BeSameAs(newModel);
        cut.Instance.Model.Name.Should().Be("Test Name");
        cut.Instance.Model.Count.Should().Be(42);
    }

    /// <summary>
    /// Verifies that setting null model creates a new default instance.
    /// </summary>
    [Fact]
    public void SetModel_Null_CreatesNewDefaultInstance()
    {
        // Arrange
        var cut = RenderComponent<Form<SimpleModel>>();
        cut.Instance.SetModel(new SimpleModel { Name = "Original" });

        // Act
        cut.Instance.SetModel(null);

        // Assert
        cut.Instance.Model.Should().NotBeNull();
        cut.Instance.Model.Name.Should().BeNull();
    }

    /// <summary>
    /// Verifies that setting empty string values is handled correctly.
    /// </summary>
    [Fact]
    public void SetModel_EmptyStringValues_HandledCorrectly()
    {
        // Arrange
        var cut = RenderComponent<Form<SimpleModel>>();
        var model = new SimpleModel { Name = "", Count = 0 };

        // Act
        cut.Instance.SetModel(model);

        // Assert
        cut.Instance.Model.Should().BeSameAs(model);
        cut.Instance.Model.Name.Should().BeEmpty();
        cut.Instance.Model.Count.Should().Be(0);
    }

    /// <summary>
    /// Verifies that Validate() returns true for models without validation attributes.
    /// </summary>
    [Fact]
    public async Task Validate_ModelWithoutAttributes_ReturnsTrue()
    {
        // Arrange
        var cut = RenderComponent<Form<SimpleModel>>();

        // Act
        var result = await cut.InvokeAsync(() => cut.Instance.Validate());

        // Assert
        result.Should().BeTrue();
        cut.Instance.IsValid.Should().BeTrue();
        cut.Instance.ValidationErrors.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that Validate() returns false and exposes errors for invalid models.
    /// </summary>
    [Fact]
    public async Task Validate_InvalidModel_ReturnsFalseAndExposesErrors()
    {
        // Arrange
        var cut = RenderComponent<Form<ValidatedModel>>();
        cut.Instance.SetModel(new ValidatedModel { Name = null, Count = -1, Age = 0 });

        // Act
        var result = await cut.InvokeAsync(() => cut.Instance.Validate());

        // Assert
        result.Should().BeFalse();
        cut.Instance.IsValid.Should().BeFalse();
        cut.Instance.ValidationErrors.Should().HaveCount(3);
        cut.Instance.ValidationErrors.Should().Contain(e => e.ErrorMessage.Contains("Name is required"));
        cut.Instance.ValidationErrors.Should().Contain(e => e.ErrorMessage.Contains("Count must be between 0 and 1000"));
        cut.Instance.ValidationErrors.Should().Contain(e => e.ErrorMessage.Contains("Age must be between 18 and 120"));
    }

    /// <summary>
    /// Verifies that Validate() returns true for valid models.
    /// </summary>
    [Fact]
    public async Task Validate_ValidModel_ReturnsTrue()
    {
        // Arrange
        var cut = RenderComponent<Form<ValidatedModel>>();
        cut.Instance.SetModel(new ValidatedModel { Name = "Valid Name", Count = 50, Age = 30 });

        // Act
        var result = await cut.InvokeAsync(() => cut.Instance.Validate());

        // Assert
        result.Should().BeTrue();
        cut.Instance.IsValid.Should().BeTrue();
        cut.Instance.ValidationErrors.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that Validate() handles boundary values correctly.
    /// </summary>
    [Fact]
    public async Task Validate_BoundaryValues_HandledCorrectly()
    {
        // Arrange
        var cut = RenderComponent<Form<ValidatedModel>>();
        cut.Instance.SetModel(new ValidatedModel { Name = new string('A', 50), Count = 0, Age = 18 });

        // Act
        var result = await cut.InvokeAsync(() => cut.Instance.Validate());

        // Assert - should be valid at boundaries
        result.Should().BeTrue();
        cut.Instance.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that Validate() handles out-of-boundary values correctly.
    /// </summary>
    [Fact]
    public async Task Validate_OutOfBoundaryValues_ReturnsFalse()
    {
        // Arrange
        var cut = RenderComponent<Form<ValidatedModel>>();
        cut.Instance.SetModel(new ValidatedModel { Name = "Valid", Count = 1001, Age = 17 });

        // Act
        var result = await cut.InvokeAsync(() => cut.Instance.Validate());

        // Assert
        result.Should().BeFalse();
        cut.Instance.IsValid.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that setting a new model after failed validation resets validation state.
    /// </summary>
    [Fact]
    public async Task SetModel_AfterFailedValidation_ResetsValidationState()
    {
        // Arrange
        var cut = RenderComponent<Form<ValidatedModel>>();
        cut.Instance.SetModel(new ValidatedModel { Name = null, Count = -1, Age = 30 });
        await cut.InvokeAsync(() => cut.Instance.Validate());

        cut.Instance.IsValid.Should().BeFalse();
        cut.Instance.ValidationErrors.Should().HaveCount(2);

        // Act
        cut.Instance.SetModel(new ValidatedModel { Name = "Valid Name", Count = 50 });

        // Assert
        cut.Instance.IsValid.Should().BeTrue();
        cut.Instance.ValidationErrors.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that Reset() clears the IsDirty flag.
    /// </summary>
    [Fact]
    public void Reset_ClearsIsDirtyFlag()
    {
        // Arrange
        var cut = RenderComponent<Form<SimpleModel>>();

        // Simulate dirty state by directly setting the field
        var field = cut.Instance.GetType().GetField("_isDirty", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(cut.Instance, true);

        // Act
        cut.Instance.Reset();

        // Assert
        cut.Instance.IsDirty.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that Reset() can be called multiple times without errors.
    /// </summary>
    [Fact]
    public void Reset_MultipleTimes_DoesNotThrow()
    {
        // Arrange
        var cut = RenderComponent<Form<SimpleModel>>();

        // Act & Assert
        cut.Instance.Reset();
        cut.Instance.Reset();
        cut.Instance.Reset();

        // Should not throw
        cut.Instance.Should().NotBeNull();
    }

    /// <summary>
    /// Verifies that Reset() maintains the model instance.
    /// </summary>
    [Fact]
    public void Reset_PreservesModelInstance()
    {
        // Arrange
        var cut = RenderComponent<Form<SimpleModel>>();
        var originalModel = cut.Instance.Model;

        // Act
        cut.Instance.Reset();

        // Assert
        cut.Instance.Model.Should().BeSameAs(originalModel);
    }

    /// <summary>
    /// Verifies that IsValid returns true when validation has not run yet.
    /// </summary>
    [Fact]
    public void IsValid_WhenValidationNotRun_ReturnsTrue()
    {
        // Arrange
        var cut = RenderComponent<Form<ValidatedModel>>();

        // Act & Assert
        cut.Instance.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that ValidationErrors is empty when validation has not run yet.
    /// </summary>
    [Fact]
    public void ValidationErrors_WhenValidationNotRun_ReturnsEmpty()
    {
        // Arrange
        var cut = RenderComponent<Form<ValidatedModel>>();

        // Act & Assert
        cut.Instance.ValidationErrors.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that complex validation with multiple attribute types works correctly.
    /// </summary>
    [Fact]
    public async Task Validate_ComplexModelWithMultipleAttributes_WorksCorrectly()
    {
        // Arrange
        var cut = RenderComponent<Form<ComplexModel>>();
        cut.Instance.SetModel(new ComplexModel {
            RequiredField = "",
            Email = "invalid-email",
            PhoneNumber = "not-a-phone"
        });

        // Act
        var result = await cut.InvokeAsync(() => cut.Instance.Validate());

        // Assert
        result.Should().BeFalse();
        cut.Instance.IsValid.Should().BeFalse();
        cut.Instance.ValidationErrors.Should().HaveCount(3);
    }

    /// <summary>
    /// Verifies that validation works with IValidatableObject implementations.
    /// </summary>
    [Fact]
    public async Task Validate_ModelWithIValidatableObject_ValidatesCustomRules()
    {
        // Arrange
        var cut = RenderComponent<Form<ValidatedModel>>();
        cut.Instance.SetModel(new ValidatedModel { Name = new string('A', 60), Count = 50, Age = 30 });

        // Act
        var result = await cut.InvokeAsync(() => cut.Instance.Validate());

        // Assert - custom validation should catch long name
        result.Should().BeFalse();
        cut.Instance.IsValid.Should().BeFalse();
        cut.Instance.ValidationErrors.Should().Contain(e => e.ErrorMessage.Contains("less than 50 characters"));
    }

    /// <summary>
    /// Verifies that the model property returns the correct instance.
    /// </summary>
    [Fact]
    public void Model_ReturnsCorrectInstance()
    {
        // Arrange
        var cut = RenderComponent<Form<SimpleModel>>();
        var testModel = new SimpleModel { Name = "Test" };
        cut.Instance.SetModel(testModel);

        // Act & Assert
        cut.Instance.Model.Should().BeSameAs(testModel);
    }

    /// <summary>
    /// Verifies that IsDirty is initially false.
    /// </summary>
    [Fact]
    public void IsDirty_InitiallyFalse()
    {
        // Arrange & Act
        var cut = RenderComponent<Form<SimpleModel>>();

        // Assert
        cut.Instance.IsDirty.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that setting a model with maximum allowed values works.
    /// </summary>
    [Fact]
    public void SetModel_MaximumValues_HandledCorrectly()
    {
        // Arrange
        var cut = RenderComponent<Form<ValidatedModel>>();
        var maxModel = new ValidatedModel { Name = new string('A', 100), Count = 1000, Age = 120 };

        // Act
        cut.Instance.SetModel(maxModel);

        // Assert
        cut.Instance.Model.Should().BeSameAs(maxModel);
        cut.Instance.Model.Name.Should().HaveLength(100);
        cut.Instance.Model.Count.Should().Be(1000);
        cut.Instance.Model.Age.Should().Be(120);
    }
}