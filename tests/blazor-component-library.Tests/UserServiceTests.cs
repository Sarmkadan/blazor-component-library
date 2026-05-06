// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Security.Cryptography;
using System.Text;
using BlazorComponentLibrary.Models;
using BlazorComponentLibrary.Repositories;
using BlazorComponentLibrary.Services;
using FluentAssertions;
using Moq;

namespace BlazorComponentLibrary.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepo;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _mockRepo = new Mock<IUserRepository>();
        _sut = new UserService(_mockRepo.Object);
    }

    [Fact]
    public async Task CreateUserAsync_PasswordTooShort_ThrowsArgumentException()
    {
        // Act
        Func<Task> act = () => _sut.CreateUserAsync("newuser", "new@test.com", "abc");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*at least 6 characters*");
    }

    [Fact]
    public async Task CreateUserAsync_UsernameAlreadyExists_ThrowsInvalidOperationException()
    {
        // Arrange
        var existing = new User
        {
            Id = 1,
            Username = "taken",
            Email = "taken@test.com",
            PasswordHash = "hash"
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User> { existing });

        // Act
        Func<Task> act = () => _sut.CreateUserAsync("taken", "other@test.com", "securePass");

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task AuthenticateAsync_ValidCredentials_ReturnsUserSummary()
    {
        // Arrange
        const string password = "SecurePass1";
        var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
        var user = new User
        {
            Id = 1,
            Username = "johndoe",
            Email = "john@example.com",
            PasswordHash = hash,
            IsActive = true
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User> { user });
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

        // Act
        var result = await _sut.AuthenticateAsync("johndoe", password);

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("johndoe");
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task AuthenticateAsync_DeactivatedAccount_ReturnsNull()
    {
        // Arrange
        const string password = "SecurePass1";
        var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
        var user = new User
        {
            Username = "suspended",
            Email = "suspended@example.com",
            PasswordHash = hash,
            IsActive = false
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User> { user });

        // Act
        var result = await _sut.AuthenticateAsync("suspended", password);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SearchUsersAsync_BlankSearchTerm_ReturnsEmptyWithoutHittingRepository()
    {
        // Act
        var result = await _sut.SearchUsersAsync("   ");

        // Assert
        result.Should().BeEmpty();
        _mockRepo.Verify(r => r.GetAllAsync(), Times.Never);
    }

    [Fact]
    public async Task GetUserStatisticsAsync_MixedUserList_CountsEachRoleAndStatusCorrectly()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Username = "admin1", Email = "a1@test.com", PasswordHash = "h", Role = UserRole.Admin,     IsActive = true  },
            new() { Username = "mod1",   Email = "m1@test.com", PasswordHash = "h", Role = UserRole.Moderator, IsActive = true  },
            new() { Username = "user1",  Email = "u1@test.com", PasswordHash = "h", Role = UserRole.User,      IsActive = true  },
            new() { Username = "user2",  Email = "u2@test.com", PasswordHash = "h", Role = UserRole.User,      IsActive = false }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        // Act
        var stats = await _sut.GetUserStatisticsAsync();

        // Assert
        stats.TotalUsers.Should().Be(4);
        stats.ActiveUsers.Should().Be(3);
        stats.InactiveUsers.Should().Be(1);
        stats.AdminCount.Should().Be(1);
        stats.ModeratorCount.Should().Be(1);
        stats.RegularUserCount.Should().Be(2);
    }
}
