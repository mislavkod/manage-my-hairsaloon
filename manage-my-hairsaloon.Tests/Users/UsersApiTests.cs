using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using manage_my_hairsaloon.Data;
using manage_my_hairsaloon.DTOs;
using manage_my_hairsaloon.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace manage_my_hairsaloon.Tests.Users;

public class UsersApiTests : IClassFixture<HairSalonApiFactory>
{
    private readonly HairSalonApiFactory _factory;
    private readonly HttpClient _client;

    public UsersApiTests(HairSalonApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ShouldReturn200WithList()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await CreateUserAsync(db);

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<UserDTO>>();
        list.Should().NotBeNull();
        list!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetById_ShouldReturn200WithDTO_WhenUserExists()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await CreateUserAsync(db);

        // Act
        var response = await _client.GetAsync($"/api/users/{user.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<UserDTO>();
        dto!.Id.Should().Be(user.Id);
        dto.Email.Should().Be(user.Email);
        dto.FirstName.Should().Be(user.FirstName);
    }

    [Fact]
    public async Task GetById_ShouldReturn404_WhenUserDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/users/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturn201WithDTO_WhenValid()
    {
        // Arrange
        var request = new UserRequest
        {
            Email = "newuser@test.com",
            FirstName = "New",
            LastName = "User",
            PhoneNumber = "555-4444",
            Role = UserRole.Customer
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await response.Content.ReadFromJsonAsync<UserDTO>();
        dto!.Email.Should().Be(request.Email);
        dto.FirstName.Should().Be(request.FirstName);
    }

    [Fact]
    public async Task Post_ShouldReturn400_WhenEmailMissing()
    {
        // Arrange — Email and FirstName/LastName are required
        var request = new { PhoneNumber = "555-0000" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_ShouldReturn200WithUpdatedDTO_WhenUserExists()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await CreateUserAsync(db);

        var request = new UserRequest
        {
            Email = "updated@test.com",
            FirstName = "Updated",
            LastName = "Name",
            Role = UserRole.Admin
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/users/{user.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<UserDTO>();
        dto!.Email.Should().Be("updated@test.com");
        dto.FirstName.Should().Be("Updated");
    }

    [Fact]
    public async Task Put_ShouldReturn404_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new UserRequest
        {
            Email = "ghost@test.com",
            FirstName = "Ghost",
            LastName = "User",
            Role = UserRole.Customer
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/users/99999", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturn204_WhenUserExists()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await CreateUserAsync(db);

        // Act
        var response = await _client.DeleteAsync($"/api/users/{user.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/users/{user.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturn404_WhenUserDoesNotExist()
    {
        // Act
        var response = await _client.DeleteAsync("/api/users/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static async Task<User> CreateUserAsync(AppDbContext db)
    {
        var user = new User
        {
            Email = $"testuser_{Guid.NewGuid()}@test.com",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "555-0001",
            Role = UserRole.Customer,
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return user;
    }
}
