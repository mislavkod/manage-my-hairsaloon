using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using manage_my_hairsaloon.Data;
using manage_my_hairsaloon.DTOs;
using manage_my_hairsaloon.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace manage_my_hairsaloon.Tests.HairSalons;

public class HairSalonsApiTests : IClassFixture<HairSalonApiFactory>
{
    private readonly HairSalonApiFactory _factory;
    private readonly HttpClient _client;

    public HairSalonsApiTests(HairSalonApiFactory factory)
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
        await CreateSalonAsync(db);

        // Act
        var response = await _client.GetAsync("/api/hairsalons");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<HairSalonDTO>>();
        list.Should().NotBeNull();
        list!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetById_ShouldReturn200WithDTO_WhenSalonExists()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var salon = await CreateSalonAsync(db);

        // Act
        var response = await _client.GetAsync($"/api/hairsalons/{salon.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<HairSalonDTO>();
        dto!.Id.Should().Be(salon.Id);
        dto.Name.Should().Be(salon.Name);
        dto.Address.Should().Be(salon.Address);
        dto.Email.Should().Be(salon.Email);
    }

    [Fact]
    public async Task GetById_ShouldReturn404_WhenSalonDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/hairsalons/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturn201WithDTO_WhenValid()
    {
        // Arrange
        var request = new HairSalonRequest
        {
            Name = "New Salon",
            Address = "New Street 5",
            PhoneNumber = "555-1111",
            Email = "new@salon.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/hairsalons", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await response.Content.ReadFromJsonAsync<HairSalonDTO>();
        dto!.Name.Should().Be(request.Name);
        dto.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task Post_ShouldReturn400_WhenNameMissing()
    {
        // Arrange — Name is required
        var request = new { Address = "Some Street" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/hairsalons", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_ShouldReturn200WithUpdatedDTO_WhenSalonExists()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var salon = await CreateSalonAsync(db);

        var request = new HairSalonRequest
        {
            Name = "Updated Salon",
            Address = "Updated Street 99",
            PhoneNumber = "555-0000",
            Email = "updated@salon.com"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/hairsalons/{salon.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<HairSalonDTO>();
        dto!.Name.Should().Be("Updated Salon");
        dto.Email.Should().Be("updated@salon.com");
    }

    [Fact]
    public async Task Put_ShouldReturn404_WhenSalonDoesNotExist()
    {
        // Arrange
        var request = new HairSalonRequest { Name = "Ghost Salon" };

        // Act
        var response = await _client.PutAsJsonAsync("/api/hairsalons/99999", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturn204_WhenSalonExists()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var salon = await CreateSalonAsync(db);

        // Act
        var response = await _client.DeleteAsync($"/api/hairsalons/{salon.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify soft-deleted
        var getResponse = await _client.GetAsync($"/api/hairsalons/{salon.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturn404_WhenSalonDoesNotExist()
    {
        // Act
        var response = await _client.DeleteAsync("/api/hairsalons/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static async Task<HairSalon> CreateSalonAsync(AppDbContext db)
    {
        var salon = new HairSalon
        {
            Name = "Test Salon",
            Address = "Test Street 1",
            PhoneNumber = "555-9999",
            Email = "test@salon.com",
            CreatedAt = DateTime.UtcNow
        };

        db.HairSalons.Add(salon);
        await db.SaveChangesAsync();

        return salon;
    }
}
