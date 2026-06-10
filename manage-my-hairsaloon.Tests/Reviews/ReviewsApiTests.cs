using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using manage_my_hairsaloon.Data;
using manage_my_hairsaloon.DTOs;
using manage_my_hairsaloon.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace manage_my_hairsaloon.Tests.Reviews;

public class ReviewsApiTests : IClassFixture<HairSalonApiFactory>
{
    private readonly HairSalonApiFactory _factory;
    private readonly HttpClient _client;

    public ReviewsApiTests(HairSalonApiFactory factory)
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
        await CreateReviewAsync(db);

        // Act
        var response = await _client.GetAsync("/api/reviews");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<ReviewDTO>>();
        list.Should().NotBeNull();
        list!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetById_ShouldReturn200WithDTO_WhenReviewExists()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var review = await CreateReviewAsync(db);

        // Act
        var response = await _client.GetAsync($"/api/reviews/{review.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<ReviewDTO>();
        dto!.Id.Should().Be(review.Id);
        dto.Rating.Should().Be(review.Rating);
        dto.Comment.Should().Be(review.Comment);
    }

    [Fact]
    public async Task GetById_ShouldReturn404_WhenReviewDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/reviews/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturn201WithDTO_WhenValid()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var (reservation, customer) = await CreateReservationAndCustomerAsync(db);

        var request = new ReviewRequest
        {
            ReservationId = reservation.Id,
            CustomerId = customer.Id,
            Rating = 5,
            Comment = "Excellent service!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reviews", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await response.Content.ReadFromJsonAsync<ReviewDTO>();
        dto!.Rating.Should().Be(5);
        dto.Comment.Should().Be("Excellent service!");
    }

    [Fact]
    public async Task Post_ShouldReturn400_WhenRatingOutOfRange()
    {
        // Arrange — Rating must be 1-5
        var request = new { ReservationId = 1, CustomerId = 1, Rating = 10, Comment = "Bad rating value" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reviews", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_ShouldReturn200WithUpdatedDTO_WhenReviewExists()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var review = await CreateReviewAsync(db);

        var request = new ReviewRequest
        {
            ReservationId = review.ReservationId,
            CustomerId = review.CustomerId,
            Rating = 2,
            Comment = "Updated comment"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/reviews/{review.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<ReviewDTO>();
        dto!.Rating.Should().Be(2);
        dto.Comment.Should().Be("Updated comment");
    }

    [Fact]
    public async Task Put_ShouldReturn404_WhenReviewDoesNotExist()
    {
        // Arrange
        var request = new ReviewRequest
        {
            ReservationId = 1,
            CustomerId = 1,
            Rating = 3,
            Comment = "Ghost review"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/reviews/99999", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturn204_WhenReviewExists()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var review = await CreateReviewAsync(db);

        // Act
        var response = await _client.DeleteAsync($"/api/reviews/{review.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/reviews/{review.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturn404_WhenReviewDoesNotExist()
    {
        // Act
        var response = await _client.DeleteAsync("/api/reviews/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static async Task<Review> CreateReviewAsync(AppDbContext db)
    {
        var (reservation, customer) = await CreateReservationAndCustomerAsync(db);

        var review = new Review
        {
            ReservationId = reservation.Id,
            CustomerId = customer.Id,
            Rating = 4,
            Comment = "Great service",
            CreatedAt = DateTime.UtcNow
        };

        db.Reviews.Add(review);
        await db.SaveChangesAsync();

        return review;
    }

    private static async Task<(Reservation reservation, User customer)> CreateReservationAndCustomerAsync(AppDbContext db)
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

        var reservation = new Reservation
        {
            CustomerId = customer.Id,
            StaffId = staff.Id,
            ServiceId = service.Id,
            ReservationDateTime = DateTime.UtcNow.AddDays(1),
            Status = ReservationStatus.Completed,
            CreatedAt = DateTime.UtcNow
        };

        db.Reservations.Add(reservation);
        await db.SaveChangesAsync();

        return (reservation, customer);
    }
}
