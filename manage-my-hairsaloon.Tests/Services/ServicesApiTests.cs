using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using manage_my_hairsaloon.Data;
using manage_my_hairsaloon.DTOs;
using manage_my_hairsaloon.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace manage_my_hairsaloon.Tests.Services;

public class ServicesApiTests : IClassFixture<HairSalonApiFactory>
{
    private readonly HairSalonApiFactory _factory;
    private readonly HttpClient _client;

    public ServicesApiTests(HairSalonApiFactory factory)
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
        await CreateServiceAsync(db);

        // Act
        var response = await _client.GetAsync("/api/services");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<ServiceDTO>>();
        list.Should().NotBeNull();
        list!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetById_ShouldReturn200WithDTO_WhenServiceExists()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var service = await CreateServiceAsync(db);

        // Act
        var response = await _client.GetAsync($"/api/services/{service.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<ServiceDTO>();
        dto!.Id.Should().Be(service.Id);
        dto.Name.Should().Be(service.Name);
        dto.Price.Should().Be(service.Price);
    }

    [Fact]
    public async Task GetById_ShouldReturn404_WhenServiceDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/services/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturn201WithDTO_WhenValid()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var salon = await CreateSalonAsync(db);

        var request = new ServiceRequest
        {
            HairSalonId = salon.Id,
            Name = "New Service",
            Description = "Test description",
            Price = 50m,
            DurationMinutes = 45,
            Category = ServiceCategory.HairCut
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/services", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await response.Content.ReadFromJsonAsync<ServiceDTO>();
        dto!.Name.Should().Be(request.Name);
        dto.Price.Should().Be(request.Price);
    }

    [Fact]
    public async Task Post_ShouldReturn400_WhenNameMissing()
    {
        // Arrange — Name is required
        var request = new { Price = 50m, DurationMinutes = 30 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/services", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_ShouldReturn200WithUpdatedDTO_WhenServiceExists()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var service = await CreateServiceAsync(db);

        var request = new ServiceRequest
        {
            HairSalonId = service.HairSalonId,
            Name = "Updated Service",
            Price = 99m,
            DurationMinutes = 60,
            Category = ServiceCategory.Coloring
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/services/{service.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<ServiceDTO>();
        dto!.Name.Should().Be("Updated Service");
        dto.Price.Should().Be(99m);
    }

    [Fact]
    public async Task Put_ShouldReturn404_WhenServiceDoesNotExist()
    {
        // Arrange
        var request = new ServiceRequest
        {
            HairSalonId = 1,
            Name = "Ghost Service",
            Price = 10m,
            DurationMinutes = 30,
            Category = ServiceCategory.HairCut
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/services/99999", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturn204_WhenServiceExists()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var service = await CreateServiceAsync(db);

        // Act
        var response = await _client.DeleteAsync($"/api/services/{service.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/services/{service.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturn404_WhenServiceDoesNotExist()
    {
        // Act
        var response = await _client.DeleteAsync("/api/services/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static async Task<Service> CreateServiceAsync(AppDbContext db)
    {
        var salon = await CreateSalonAsync(db);

        var service = new Service
        {
            HairSalonId = salon.Id,
            Name = "Test Service",
            Description = "Test description",
            Price = 50m,
            DurationMinutes = 45,
            Category = ServiceCategory.HairCut
        };

        db.Services.Add(service);
        await db.SaveChangesAsync();

        return service;
    }

    private static async Task<HairSalon> CreateSalonAsync(AppDbContext db)
    {
        var salon = new HairSalon
        {
            Name = $"Salon_{Guid.NewGuid()}",
            CreatedAt = DateTime.UtcNow
        };

        db.HairSalons.Add(salon);
        await db.SaveChangesAsync();

        return salon;
    }
}
