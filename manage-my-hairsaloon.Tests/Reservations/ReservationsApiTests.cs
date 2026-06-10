using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using manage_my_hairsaloon.Data;
using manage_my_hairsaloon.DTOs;
using manage_my_hairsaloon.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace manage_my_hairsaloon.Tests.Reservations;

public class ReservationsApiTests : IClassFixture<HairSalonApiFactory>
{
    private readonly HairSalonApiFactory _factory;
    private readonly HttpClient _client;

    public ReservationsApiTests(HairSalonApiFactory factory)
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
        await CreateReservationAsync(db);

        // Act
        var response = await _client.GetAsync("/api/reservations");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<ReservationDTO>>();
        list.Should().NotBeNull();
        list!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetById_ShouldReturn200WithDTO_WhenReservationExists()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var reservation = await CreateReservationAsync(db);

        // Act
        var response = await _client.GetAsync($"/api/reservations/{reservation.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<ReservationDTO>();
        dto!.Id.Should().Be(reservation.Id);
        dto.Status.Should().Be(reservation.Status.ToString());
    }

    [Fact]
    public async Task GetById_ShouldReturn404_WhenReservationDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/reservations/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturn201WithDTO_WhenValid()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var (customer, staff, service) = await CreateDependenciesAsync(db);

        var request = new ReservationRequest
        {
            CustomerId = customer.Id,
            StaffId = staff.Id,
            ServiceId = service.Id,
            ReservationDateTime = DateTime.UtcNow.AddDays(1),
            Notes = "Test notes"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reservations", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await response.Content.ReadFromJsonAsync<ReservationDTO>();
        dto!.Status.Should().Be("Pending");
        dto.Notes.Should().Be("Test notes");
    }

    [Fact]
    public async Task Delete_ShouldReturn204_WhenReservationExists()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var reservation = await CreateReservationAsync(db);

        // Act
        var response = await _client.DeleteAsync($"/api/reservations/{reservation.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/reservations/{reservation.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturn404_WhenReservationDoesNotExist()
    {
        // Act
        var response = await _client.DeleteAsync("/api/reservations/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static async Task<Reservation> CreateReservationAsync(AppDbContext db)
    {
        var (customer, staff, service) = await CreateDependenciesAsync(db);

        var reservation = new Reservation
        {
            CustomerId = customer.Id,
            StaffId = staff.Id,
            ServiceId = service.Id,
            ReservationDateTime = DateTime.UtcNow.AddDays(1),
            Status = ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        db.Reservations.Add(reservation);
        await db.SaveChangesAsync();

        return reservation;
    }

    private static async Task<(User customer, Models.Staff staff, Service service)> CreateDependenciesAsync(AppDbContext db)
    {
        var customer = new User
        {
            Email = $"customer_{Guid.NewGuid()}@test.com",
            FirstName = "Customer",
            LastName = "Test",
            Role = UserRole.Customer,
            CreatedAt = DateTime.UtcNow
        };

        var staffUser = new User
        {
            Email = $"staff_{Guid.NewGuid()}@test.com",
            FirstName = "Staff",
            LastName = "Test",
            Role = UserRole.Staff,
            CreatedAt = DateTime.UtcNow
        };

        var salon = new HairSalon
        {
            Name = $"Salon_{Guid.NewGuid()}",
            CreatedAt = DateTime.UtcNow
        };

        db.Users.AddRange(customer, staffUser);
        db.HairSalons.Add(salon);
        await db.SaveChangesAsync();

        var staff = new Models.Staff
        {
            UserId = staffUser.Id,
            HairSalonId = salon.Id,
            Specialization = "Test",
            HourlyRate = 40m,
            IsAvailable = true
        };

        var service = new Service
        {
            HairSalonId = salon.Id,
            Name = "Test Service",
            Price = 50m,
            DurationMinutes = 45,
            Category = ServiceCategory.HairCut
        };

        db.Staff.Add(staff);
        db.Services.Add(service);
        await db.SaveChangesAsync();

        return (customer, staff, service);
    }
}
