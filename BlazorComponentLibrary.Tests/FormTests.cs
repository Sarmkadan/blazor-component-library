using BlazorComponentLibrary.Components.Form;
using FluentAssertions;
using Xunit;

namespace BlazorComponentLibrary.Tests;

public class FormTests
{
    private class TestModel
    {
        public string? Name { get; set; }
    }

    private class TestableForm<T> : Form<T> where T : new()
    {
        protected override void NotifyStateChanged() { }
    }

    [Fact]
    public void SetModel_ShouldUpdateModel()
    {
        // Arrange
        var form = new TestableForm<TestModel>();
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
        var form = new TestableForm<TestModel>();

        // Assert
        form.Model.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Validate_ShouldAlwaysReturnTrue()
    {
        // Arrange
        var form = new TestableForm<TestModel>();

        // Act
        var result = await form.Validate();

        // Assert
        result.Should().BeTrue();
    }
}
