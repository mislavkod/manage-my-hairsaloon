using manage_my_hairsaloon.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// ===== SAMPLE DATA =====

// Create Users (Customers and Staff)
var customer1 = new User
{
    Id = 1,
    Email = "john@email.com",
    PasswordHash = "hash123",
    FirstName = "John",
    LastName = "Doe",
    PhoneNumber = "555-0101",
    Role = UserRole.Customer,
    CreatedAt = DateTime.Now
};

var customer2 = new User
{
    Id = 2,
    Email = "jane@email.com",
    PasswordHash = "hash456",
    FirstName = "Jane",
    LastName = "Smith",
    PhoneNumber = "555-0102",
    Role = UserRole.Customer,
    CreatedAt = DateTime.Now
};

var customer3 = new User
{
    Id = 3,
    Email = "mike@email.com",
    PasswordHash = "hash789",
    FirstName = "Mike",
    LastName = "Johnson",
    PhoneNumber = "555-0103",
    Role = UserRole.Customer,
    CreatedAt = DateTime.Now
};

var staff1 = new User
{
    Id = 4,
    Email = "anna@salon.com",
    PasswordHash = "staffhash1",
    FirstName = "Anna",
    LastName = "Wilson",
    PhoneNumber = "555-0201",
    Role = UserRole.Staff,
    CreatedAt = DateTime.Now
};

var staff2 = new User
{
    Id = 5,
    Email = "bob@salon.com",
    PasswordHash = "staffhash2",
    FirstName = "Bob",
    LastName = "Brown",
    PhoneNumber = "555-0202",
    Role = UserRole.Staff,
    CreatedAt = DateTime.Now
};

var staff3 = new User
{
    Id = 6,
    Email = "sarah@salon.com",
    PasswordHash = "staffhash3",
    FirstName = "Sarah",
    LastName = "Davis",
    PhoneNumber = "555-0203",
    Role = UserRole.Staff,
    CreatedAt = DateTime.Now
};

var staff4 = new User
{
    Id = 7,
    Email = "tom@salon.com",
    PasswordHash = "staffhash4",
    FirstName = "Tom",
    LastName = "Miller",
    PhoneNumber = "555-0204",
    Role = UserRole.Staff,
    CreatedAt = DateTime.Now
};

var staff5 = new User
{
    Id = 8,
    Email = "lucy@salon.com",
    PasswordHash = "staffhash5",
    FirstName = "Lucy",
    LastName = "Taylor",
    PhoneNumber = "555-0205",
    Role = UserRole.Staff,
    CreatedAt = DateTime.Now
};

// Create Hair Salons
var salon1 = new HairSalon
{
    Id = 1,
    Name = "Premium Cuts Downtown",
    Address = "123 Main Street, Downtown",
    PhoneNumber = "555-1000",
    Email = "info@premiumcuts.com",
    CreatedAt = DateTime.Now
};

var salon2 = new HairSalon
{
    Id = 2,
    Name = "Beauty Oasis Mall",
    Address = "456 Mall Drive, Shopping Center",
    PhoneNumber = "555-2000",
    Email = "contact@beautyoasis.com",
    CreatedAt = DateTime.Now
};

var salon3 = new HairSalon
{
    Id = 3,
    Name = "Elegant Styles Uptown",
    Address = "789 Park Avenue, Uptown",
    PhoneNumber = "555-3000",
    Email = "service@elegantstyles.com",
    CreatedAt = DateTime.Now
};

// Create Staff for Salon 1
var staffSalon1_1 = new Staff
{
    Id = 1,
    UserId = 4,
    HairSalonId = 1,
    Specialization = "Hair Coloring",
    HourlyRate = 50m,
    IsAvailable = true,
    User = staff1,
    HairSalon = salon1
};

var staffSalon1_2 = new Staff
{
    Id = 2,
    UserId = 5,
    HairSalonId = 1,
    Specialization = "Hair Cut & Styling",
    HourlyRate = 45m,
    IsAvailable = true,
    User = staff2,
    HairSalon = salon1
};

var staffSalon2_1 = new Staff
{
    Id = 3,
    UserId = 6,
    HairSalonId = 2,
    Specialization = "Beard Grooming",
    HourlyRate = 40m,
    IsAvailable = true,
    User = staff3,
    HairSalon = salon2
};

var staffSalon2_2 = new Staff
{
    Id = 4,
    UserId = 7,
    HairSalonId = 2,
    Specialization = "Nails & Extensions",
    HourlyRate = 35m,
    IsAvailable = true,
    User = staff4,
    HairSalon = salon2
};

var staffSalon3_1 = new Staff
{
    Id = 5,
    UserId = 8,
    HairSalonId = 3,
    Specialization = "Facial Treatments",
    HourlyRate = 55m,
    IsAvailable = true,
    User = staff5,
    HairSalon = salon3
};

salon1.Staff.Add(staffSalon1_1);
salon1.Staff.Add(staffSalon1_2);
salon2.Staff.Add(staffSalon2_1);
salon2.Staff.Add(staffSalon2_2);
salon3.Staff.Add(staffSalon3_1);

// Create Services for Salon 1
var service1 = new Service
{
    Id = 1,
    HairSalonId = 1,
    Name = "Premium Hair Cut",
    Description = "Professional haircut with styling",
    Price = 60m,
    DurationMinutes = 45,
    Category = ServiceCategory.HairCut,
    HairSalon = salon1
};

var service2 = new Service
{
    Id = 2,
    HairSalonId = 1,
    Name = "Hair Coloring",
    Description = "Full head color treatment",
    Price = 120m,
    DurationMinutes = 120,
    Category = ServiceCategory.Coloring,
    HairSalon = salon1
};

var service3 = new Service
{
    Id = 3,
    HairSalonId = 1,
    Name = "Hair Treatment",
    Description = "Deep conditioning treatment",
    Price = 75m,
    DurationMinutes = 60,
    Category = ServiceCategory.Treatment,
    HairSalon = salon1
};

var service4 = new Service
{
    Id = 4,
    HairSalonId = 2,
    Name = "Beard Trim",
    Description = "Professional beard shaping and trim",
    Price = 35m,
    DurationMinutes = 30,
    Category = ServiceCategory.Beard,
    HairSalon = salon2
};

var service5 = new Service
{
    Id = 5,
    HairSalonId = 2,
    Name = "Nail Manicure",
    Description = "Full manicure with polish",
    Price = 40m,
    DurationMinutes = 45,
    Category = ServiceCategory.Nails,
    HairSalon = salon2
};

var service6 = new Service
{
    Id = 6,
    HairSalonId = 2,
    Name = "Styling",
    Description = "Professional hair styling",
    Price = 50m,
    DurationMinutes = 50,
    Category = ServiceCategory.Styling,
    HairSalon = salon2
};

var service7 = new Service
{
    Id = 7,
    HairSalonId = 3,
    Name = "Facial Treatment Deluxe",
    Description = "Premium facial with all treatments",
    Price = 100m,
    DurationMinutes = 90,
    Category = ServiceCategory.FacialTreatment,
    HairSalon = salon3
};

var service8 = new Service
{
    Id = 8,
    HairSalonId = 3,
    Name = "Hair Styling Pro",
    Description = "Advanced styling techniques",
    Price = 85m,
    DurationMinutes = 75,
    Category = ServiceCategory.Styling,
    HairSalon = salon3
};

var service9 = new Service
{
    Id = 9,
    HairSalonId = 3,
    Name = "Hair Treatment Premium",
    Description = "Luxury hair restoration",
    Price = 150m,
    DurationMinutes = 90,
    Category = ServiceCategory.Treatment,
    HairSalon = salon3
};

salon1.Services.AddRange(new[] { service1, service2, service3 });
salon2.Services.AddRange(new[] { service4, service5, service6 });
salon3.Services.AddRange(new[] { service7, service8, service9 });

// Create Reservations
var reservation1 = new Reservation
{
    Id = 1,
    CustomerId = 1,
    StaffId = 1,
    ServiceId = 1,
    ReservationDateTime = new DateTime(2026, 4, 5, 10, 0, 0),
    Status = ReservationStatus.Confirmed,
    Notes = "First time customer, prefers fade cut",
    CreatedAt = DateTime.Now,
    Customer = customer1,
    Staff = staffSalon1_1,
    Service = service1
};

var reservation2 = new Reservation
{
    Id = 2,
    CustomerId = 2,
    StaffId = 2,
    ServiceId = 2,
    ReservationDateTime = new DateTime(2026, 4, 6, 14, 0, 0),
    Status = ReservationStatus.Confirmed,
    Notes = "Wants ash blonde color",
    CreatedAt = DateTime.Now,
    Customer = customer2,
    Staff = staffSalon1_2,
    Service = service2
};

var reservation3 = new Reservation
{
    Id = 3,
    CustomerId = 3,
    StaffId = 2,
    ServiceId = 3,
    ReservationDateTime = new DateTime(2026, 4, 7, 11, 0, 0),
    Status = ReservationStatus.Pending,
    Notes = "Dry hair needs treatment",
    CreatedAt = DateTime.Now,
    Customer = customer3,
    Staff = staffSalon1_2,
    Service = service3
};

var reservation4 = new Reservation
{
    Id = 4,
    CustomerId = 1,
    StaffId = 3,
    ServiceId = 4,
    ReservationDateTime = new DateTime(2026, 4, 8, 9, 0, 0),
    Status = ReservationStatus.Completed,
    Notes = "Regular customer",
    CreatedAt = DateTime.Now,
    Customer = customer1,
    Staff = staffSalon2_1,
    Service = service4
};

var reservation5 = new Reservation
{
    Id = 5,
    CustomerId = 2,
    StaffId = 4,
    ServiceId = 5,
    ReservationDateTime = new DateTime(2026, 4, 9, 15, 30, 0),
    Status = ReservationStatus.Confirmed,
    Notes = "Gel manicure preferred",
    CreatedAt = DateTime.Now,
    Customer = customer2,
    Staff = staffSalon2_2,
    Service = service5
};

var reservation6 = new Reservation
{
    Id = 6,
    CustomerId = 3,
    StaffId = 4,
    ServiceId = 6,
    ReservationDateTime = new DateTime(2026, 4, 10, 13, 0, 0),
    Status = ReservationStatus.Pending,
    Notes = "Wedding event styling",
    CreatedAt = DateTime.Now,
    Customer = customer3,
    Staff = staffSalon2_2,
    Service = service6
};

var reservation7 = new Reservation
{
    Id = 7,
    CustomerId = 1,
    StaffId = 5,
    ServiceId = 7,
    ReservationDateTime = new DateTime(2026, 4, 11, 10, 0, 0),
    Status = ReservationStatus.Confirmed,
    Notes = "Sensitive skin",
    CreatedAt = DateTime.Now,
    Customer = customer1,
    Staff = staffSalon3_1,
    Service = service7
};

var reservation8 = new Reservation
{
    Id = 8,
    CustomerId = 2,
    StaffId = 5,
    ServiceId = 8,
    ReservationDateTime = new DateTime(2026, 4, 12, 11, 0, 0),
    Status = ReservationStatus.Confirmed,
    Notes = "Special occasion",
    CreatedAt = DateTime.Now,
    Customer = customer2,
    Staff = staffSalon3_1,
    Service = service8
};

var reservation9 = new Reservation
{
    Id = 9,
    CustomerId = 3,
    StaffId = 5,
    ServiceId = 9,
    ReservationDateTime = new DateTime(2026, 4, 13, 14, 0, 0),
    Status = ReservationStatus.Cancelled,
    Notes = "Customer cancelled",
    CreatedAt = DateTime.Now,
    Customer = customer3,
    Staff = staffSalon3_1,
    Service = service9
};

staffSalon1_1.Reservations.Add(reservation1);
staffSalon1_2.Reservations.AddRange(new[] { reservation2, reservation3 });
staffSalon2_1.Reservations.Add(reservation4);
staffSalon2_2.Reservations.AddRange(new[] { reservation5, reservation6 });
staffSalon3_1.Reservations.AddRange(new[] { reservation7, reservation8, reservation9 });

var salons = new List<HairSalon> { salon1, salon2, salon3 };
var reservations = new List<Reservation> { reservation1, reservation2, reservation3, reservation4, reservation5, reservation6, reservation7, reservation8, reservation9 };

// ===== LINQ QUERIES =====

Console.WriteLine("=== HAIR SALON MANAGEMENT SYSTEM ===\n");

// 1. Get all confirmed reservations
var confirmedReservations = reservations
    .Where(r => r.Status == ReservationStatus.Confirmed)
    .ToList();

Console.WriteLine($"✓ Confirmed Reservations: {confirmedReservations.Count}");
foreach (var res in confirmedReservations)
{
    Console.WriteLine($"  - {res.Customer.FirstName} on {res.ReservationDateTime:yyyy-MM-dd}");
}
Console.WriteLine();

// 2. Get all reservations sorted by date
var reservationsByDate = reservations
    .OrderBy(r => r.ReservationDateTime)
    .ToList();

Console.WriteLine($"📅 Reservations by Date:");
foreach (var res in reservationsByDate.Take(3))
{
    Console.WriteLine($"  - {res.ReservationDateTime:yyyy-MM-dd HH:mm}: {res.Service.Name}");
}
Console.WriteLine();

// 3. Find reservations for a specific customer
var customerReservations = reservations
    .Where(r => r.CustomerId == 1)
    .Select(r => new { r.Service.Name, r.ReservationDateTime, r.Status })
    .ToList();

Console.WriteLine($"👤 John Doe's Reservations: {customerReservations.Count}");
foreach (var res in customerReservations)
{
    Console.WriteLine($"  - {res.Name} ({res.Status})");
}
Console.WriteLine();

// 4. Get staff members by salon
var premiumCutsStaff = salons
    .FirstOrDefault(s => s.Name == "Premium Cuts Downtown")
    ?.Staff
    .OrderBy(s => s.Specialization)
    .ToList();

Console.WriteLine($"👥 Premium Cuts Downtown Staff:");
if (premiumCutsStaff != null)
{
    foreach (var staff in premiumCutsStaff)
    {
        Console.WriteLine($"  - {staff.User.FirstName} ({staff.Specialization})");
    }
}
Console.WriteLine();

// 5. Find the most expensive service
var mostExpensiveService = salons
    .SelectMany(s => s.Services)
    .OrderByDescending(s => s.Price)
    .First();

Console.WriteLine($"💎 Most Expensive Service: {mostExpensiveService.Name} - ${mostExpensiveService.Price}\n");

// 6. Count reservations per staff member
var reservationsPerStaff = reservations
    .GroupBy(r => r.Staff.User.FirstName)
    .Select(g => new { StaffName = g.Key, Count = g.Count() })
    .OrderByDescending(x => x.Count)
    .ToList();

Console.WriteLine($"📊 Reservations per Staff Member:");
foreach (var staff in reservationsPerStaff)
{
    Console.WriteLine($"  - {staff.StaffName}: {staff.Count} reservations");
}
Console.WriteLine();

// 7. Get all services under $60
var affordableServices = salons
    .SelectMany(s => s.Services)
    .Where(s => s.Price < 60)
    .Select(s => new { s.Name, s.Price, s.Category })
    .ToList();

Console.WriteLine($"💰 Services Under $60: {affordableServices.Count}");
foreach (var service in affordableServices)
{
    Console.WriteLine($"  - {service.Name}: ${service.Price}");
}
Console.WriteLine();

// 8. Find pending reservations that need confirmation
var pendingReservations = reservations
    .Where(r => r.Status == ReservationStatus.Pending)
    .OrderBy(r => r.ReservationDateTime)
    .ToList();

Console.WriteLine($"⏳ Pending Reservations: {pendingReservations.Count}");
foreach (var res in pendingReservations)
{
    Console.WriteLine($"  - {res.Customer.FirstName} - {res.Service.Name}");
}
Console.WriteLine();

// 9. Get services by category
var servicesByCategory = salons
    .SelectMany(s => s.Services)
    .GroupBy(s => s.Category)
    .Select(g => new { Category = g.Key, ServiceCount = g.Count() })
    .ToList();

Console.WriteLine($"🏷️  Services by Category:");
foreach (var cat in servicesByCategory)
{
    Console.WriteLine($"  - {cat.Category}: {cat.ServiceCount} services");
}
Console.WriteLine();

// 10. Get average service price per salon
var avgPricePerSalon = salons
    .Select(s => new 
    { 
        SalonName = s.Name, 
        AveragePrice = s.Services.Any() ? s.Services.Average(srv => srv.Price) : 0 
    })
    .ToList();

Console.WriteLine($"📈 Average Service Price per Salon:");
foreach (var salon in avgPricePerSalon)
{
    Console.WriteLine($"  - {salon.SalonName}: ${salon.AveragePrice:F2}");
}
Console.WriteLine();

// 11. Find top 3 staff with highest hourly rate
var topStaff = salons
    .SelectMany(s => s.Staff)
    .OrderByDescending(s => s.HourlyRate)
    .Take(3)
    .Select(s => new { s.User.FirstName, s.Specialization, s.HourlyRate })
    .ToList();

Console.WriteLine($"⭐ Top 3 Best Paid Staff:");
foreach (var staff in topStaff)
{
    Console.WriteLine($"  - {staff.FirstName} ({staff.Specialization}): ${staff.HourlyRate}/hr");
}
Console.WriteLine();

// 12. Count completed reservations
var completedCount = reservations
    .Where(r => r.Status == ReservationStatus.Completed)
    .Count();

Console.WriteLine($"✅ Total Completed Reservations: {completedCount}\n");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
