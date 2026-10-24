namespace BlazorComponentLibrary.Tests;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BlazorComponentLibrary.Components;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Xunit;

/// <summary>
/// Architecture tests that reflect over every public component type and enforce the shared parameter
/// conventions defined by <see cref="LibraryComponentBase"/>, so that new components cannot drift.
/// </summary>
public sealed class ComponentConventionTests
{
    private const string ChangedSuffix = "Changed";

    private const BindingFlags InstanceProperties = BindingFlags.Public | BindingFlags.Instance;

    private const BindingFlags DeclaredInstanceProperties =
        BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private static readonly IReadOnlyList<Type> PublicComponents = typeof(LibraryComponentBase).Assembly
        .GetExportedTypes()
        .Where(t => t != typeof(LibraryComponentBase) && typeof(ComponentBase).IsAssignableFrom(t))
        .ToList();

    /// <summary>
    /// Verifies that reflection discovers the public components, so the convention checks below cannot pass vacuously.
    /// </summary>
    [Fact]
    public void PublicComponents_AreDiscovered()
    {
        // Arrange
        // (the component list is built once from the library assembly)

        // Act
        var count = PublicComponents.Count;

        // Assert
        count.Should().BeGreaterThan(0);
    }

    /// <summary>
    /// Verifies that every public component derives from <see cref="LibraryComponentBase"/> and therefore inherits the shared parameters.
    /// </summary>
    [Fact]
    public void PublicComponents_DeriveFromLibraryComponentBase()
    {
        // Arrange
        // (the component list is built once from the library assembly)

        // Act
        var violations = PublicComponents
            .Where(t => !typeof(LibraryComponentBase).IsAssignableFrom(t))
            .Select(t => $"{t.FullName} does not derive from LibraryComponentBase")
            .ToList();

        // Assert
        violations.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that <see cref="LibraryComponentBase"/> declares <c>Class</c> as a nullable string parameter and
    /// <c>AdditionalAttributes</c> as a dictionary parameter with <c>CaptureUnmatchedValues</c> enabled.
    /// </summary>
    [Fact]
    public void LibraryComponentBase_DeclaresSharedParameters()
    {
        // Arrange
        var nullability = new NullabilityInfoContext();
        var classProperty = typeof(LibraryComponentBase).GetProperty(nameof(LibraryComponentBase.Class), InstanceProperties);
        var attributesProperty = typeof(LibraryComponentBase).GetProperty(nameof(LibraryComponentBase.AdditionalAttributes), InstanceProperties);

        // Act
        var classIsNullableString = classProperty is not null
            && classProperty.PropertyType == typeof(string)
            && nullability.Create(classProperty).WriteState == NullabilityState.Nullable;
        var classIsParameter = classProperty?.GetCustomAttribute<ParameterAttribute>() is not null;
        var attributesIsDictionary = attributesProperty?.PropertyType == typeof(IDictionary<string, object>);
        var attributesCaptures = attributesProperty?.GetCustomAttribute<ParameterAttribute>()?.CaptureUnmatchedValues == true;

        // Assert
        classIsNullableString.Should().BeTrue("Class must be string?");
        classIsParameter.Should().BeTrue("Class must be a [Parameter]");
        attributesIsDictionary.Should().BeTrue("AdditionalAttributes must be IDictionary<string, object>?");
        attributesCaptures.Should().BeTrue("AdditionalAttributes must be [Parameter(CaptureUnmatchedValues = true)]");
    }

    /// <summary>
    /// Verifies that no component redeclares <c>Class</c> or <c>AdditionalAttributes</c>, which would shadow the shared behaviour.
    /// </summary>
    [Fact]
    public void PublicComponents_DoNotRedeclareSharedParameters()
    {
        // Arrange
        var sharedNames = new HashSet<string> { nameof(LibraryComponentBase.Class), nameof(LibraryComponentBase.AdditionalAttributes) };

        // Act
        var violations = PublicComponents
            .SelectMany(t => t.GetProperties(DeclaredInstanceProperties)
                .Where(p => sharedNames.Contains(p.Name))
                .Select(p => $"{t.FullName}.{p.Name} redeclares a shared parameter"))
            .ToList();

        // Assert
        violations.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that no component exposes a <c>CssClass</c> parameter, because the shared parameter is named <c>Class</c>.
    /// </summary>
    [Fact]
    public void PublicComponents_DoNotExposeCssClassParameter()
    {
        // Arrange
        // (the component list is built once from the library assembly)

        // Act
        var violations = PublicComponents
            .SelectMany(t => t.GetProperties(InstanceProperties)
                .Where(p => p.Name == "CssClass" && IsParameter(p))
                .Select(p => $"{t.FullName}.{p.Name}"))
            .ToList();

        // Assert
        violations.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that every generic <c>EventCallback&lt;T&gt;</c> parameter named <c>XChanged</c> pairs with a
    /// <c>[Parameter]</c> named <c>X</c> of type <c>T</c>, so that <c>@bind-X</c> is supported.
    /// </summary>
    [Fact]
    public void PublicComponents_ChangedCallbacksPairWithValueParameter()
    {
        // Arrange
        // (the component list is built once from the library assembly)

        // Act
        var violations = PublicComponents
            .SelectMany(t => t.GetProperties(InstanceProperties)
                .Where(p => IsParameter(p) && p.Name.EndsWith(ChangedSuffix, StringComparison.Ordinal) && IsGenericEventCallback(p.PropertyType))
                .Where(callback => !HasMatchingValueParameter(t, callback))
                .Select(callback => $"{t.FullName}.{callback.Name} has no matching [Parameter] {callback.Name[..^ChangedSuffix.Length]} of type {callback.PropertyType.GetGenericArguments()[0]}"))
            .ToList();

        // Assert
        violations.Should().BeEmpty();
    }

    private static bool IsParameter(PropertyInfo property) =>
        property.GetCustomAttribute<ParameterAttribute>() is not null;

    private static bool IsGenericEventCallback(Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(EventCallback<>);

    private static bool HasMatchingValueParameter(Type component, PropertyInfo callback)
    {
        var valueName = callback.Name[..^ChangedSuffix.Length];
        var valueProperty = component.GetProperty(valueName, InstanceProperties);
        var expectedType = callback.PropertyType.GetGenericArguments()[0];

        return valueProperty is not null
            && IsParameter(valueProperty)
            && valueProperty.PropertyType == expectedType;
    }
}
