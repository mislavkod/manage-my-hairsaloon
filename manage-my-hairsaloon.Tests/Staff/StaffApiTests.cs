using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using manage_my_hairsaloon.Data;
using manage_my_hairsaloon.DTOs;
using manage_my_hairsaloon.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace manage_my_hairsaloon.Tests.Staff;

public class StaffApiTests : IClassFixture<HairSalonApiFactory>
{
    private readonly HairSalonApiFactory _factory;
    private readonly HttpClient _client;

    public StaffApiTests(HairSalonApiFactory factory)
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
        await CreateStaffAsync(db);

        // Act
        var response = await _client.GetAsync("/api/staff");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<StaffDTO>>();
        list.Should().NotBeNull();
        list!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetById_ShouldReturn200WithDTO_WhenStaffExists()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var staff = await CreateStaffAsync(db);

        // Act
        var response = await _client.GetAsync($"/api/staff/{staff.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<StaffDTO>();
        dto!.Id.Should().Be(staff.Id);
        dto.Specialization.Should().Be(staff.Specialization);
        dto.HourlyRate.Should().Be(staff.HourlyRate);
    }

    [Fact]
    public async Task GetById_ShouldReturn404_WhenStaffDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/staff/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturn201WithDTO_WhenValid()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var (user, salon) = await CreateUserAndSalonAsync(db);

        var request = new StaffRequest
        {
            UserId = user.Id,
            HairSalonId = salon.Id,
            Specialization = "Hair Styling",
            HourlyRate = 45m,
            IsAvailable = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/staff", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await response.Content.ReadFromJsonAsync<StaffDTO>();
        dto!.Specialization.Should().Be(request.Specialization);
        dto.HourlyRate.Should().Be(request.HourlyRate);
    }

    [Fact]
    public async Task Post_ShouldReturn400_WhenSpecializationMissing()
    {
        // Arrange — Specialization is required
        var request = new { HourlyRate = 40m, IsAvailable = true };

        // Act
        var response = await _client.PostAsJsonAsync("/api/staff", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_ShouldReturn200WithUpdatedDTO_WhenStaffExists()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var staff = await CreateStaffAsync(db);

        var request = new StaffRequest
        {
            UserId = staff.UserId,
            HairSalonId = staff.HairSalonId,
            Specialization = "Updated Specialization",
            HourlyRate = 60m,
            IsAvailable = false
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/staff/{staff.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<StaffDTO>();
        dto!.Specialization.Should().Be("Updated Specialization");
        dto.HourlyRate.Should().Be(60m);
        dto.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public async Task Put_ShouldReturn404_WhenStaffDoesNotExist()
    {
        // Arrange
        var request = new StaffRequest
        {
            UserId = 1,
            HairSalonId = 1,
            Specialization = "Ghost",
            HourlyRate = 30m
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/staff/99999", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturn204_WhenStaffExists()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var staff = await CreateStaffAsync(db);

        // Act
        var response = await _client.DeleteAsync($"/api/staff/{staff.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/staff/{staff.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturn404_WhenStaffDoesNotExist()
    {
        // Act
        var response = await _client.DeleteAsync("/api/staff/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static async Task<Models.Staff> CreateStaffAsync(AppDbContext db)
    {
        var (user, salon) = await CreateUserAndSalonAsync(db);

        var staff = new Models.Staff
        {
            UserId = user.Id,
            HairSalonId = salon.Id,
            Specialization = "Test Specialization",
            HourlyRate = 40m,
            IsAvailable = true
        };

        db.Staff.Add(staff);
        await db.SaveChangesAsync();

        return staff;
    }

    private static async Task<(User user, HairSalon salon)> CreateUserAndSalonAsync(AppDbContext db)
    {
        var user = new User
        {
            Email = $"staff_{Guid.NewGuid()}@test.com",
            FirstName = "Staff",
            LastName = "Member",
            Role = UserRole.Staff,
            CreatedAt = DateTime.UtcNow
        };

        var salon = new HairSalon
        {
            Name = $"Salon_{Guid.NewGuid()}",
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        db.HairSalons.Add(salon);
        await db.SaveChangesAsync();

        return (user, salon);
    }
}
