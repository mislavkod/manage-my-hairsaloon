using manage_my_hairsaloon.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace manage_my_hairsaloon.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Database.IsRelational())
                context.Database.Migrate();
            else
                context.Database.EnsureCreated();

            if (context.BusinessUsers.Any()) return;

            // ===== USERS =====
            var customer1 = new User { Email = "john@email.com",   PasswordHash = "hash123",    FirstName = "John",  LastName = "Doe",     PhoneNumber = "555-0101", Role = UserRole.Customer, CreatedAt = DateTime.UtcNow };
            var customer2 = new User { Email = "jane@email.com",   PasswordHash = "hash456",    FirstName = "Jane",  LastName = "Smith",   PhoneNumber = "555-0102", Role = UserRole.Customer, CreatedAt = DateTime.UtcNow };
            var customer3 = new User { Email = "mike@email.com",   PasswordHash = "hash789",    FirstName = "Mike",  LastName = "Johnson", PhoneNumber = "555-0103", Role = UserRole.Customer, CreatedAt = DateTime.UtcNow };
            var staffUser1 = new User { Email = "anna@salon.com",  PasswordHash = "staffhash1", FirstName = "Anna",  LastName = "Wilson",  PhoneNumber = "555-0201", Role = UserRole.Staff,    CreatedAt = DateTime.UtcNow };
            var staffUser2 = new User { Email = "bob@salon.com",   PasswordHash = "staffhash2", FirstName = "Bob",   LastName = "Brown",   PhoneNumber = "555-0202", Role = UserRole.Staff,    CreatedAt = DateTime.UtcNow };
            var staffUser3 = new User { Email = "sarah@salon.com", PasswordHash = "staffhash3", FirstName = "Sarah", LastName = "Davis",   PhoneNumber = "555-0203", Role = UserRole.Staff,    CreatedAt = DateTime.UtcNow };
            var staffUser4 = new User { Email = "tom@salon.com",   PasswordHash = "staffhash4", FirstName = "Tom",   LastName = "Miller",  PhoneNumber = "555-0204", Role = UserRole.Staff,    CreatedAt = DateTime.UtcNow };
            var staffUser5 = new User { Email = "lucy@salon.com",  PasswordHash = "staffhash5", FirstName = "Lucy",  LastName = "Taylor",  PhoneNumber = "555-0205", Role = UserRole.Staff,    CreatedAt = DateTime.UtcNow };

            context.BusinessUsers.AddRange(customer1, customer2, customer3, staffUser1, staffUser2, staffUser3, staffUser4, staffUser5);
            context.SaveChanges();

            // ===== HAIR SALONS =====
            var salon1 = new HairSalon { Name = "Premium Cuts Downtown", Address = "123 Main Street, Downtown",       PhoneNumber = "555-1000", Email = "info@premiumcuts.com",     CreatedAt = DateTime.UtcNow };
            var salon2 = new HairSalon { Name = "Beauty Oasis Mall",     Address = "456 Mall Drive, Shopping Center", PhoneNumber = "555-2000", Email = "contact@beautyoasis.com",   CreatedAt = DateTime.UtcNow };
            var salon3 = new HairSalon { Name = "Elegant Styles Uptown", Address = "789 Park Avenue, Uptown",         PhoneNumber = "555-3000", Email = "service@elegantstyles.com", CreatedAt = DateTime.UtcNow };

            context.HairSalons.AddRange(salon1, salon2, salon3);
            context.SaveChanges();

            // ===== STAFF =====
            var staff1 = new Staff { UserId = staffUser1.Id, HairSalonId = salon1.Id, Specialization = "Hair Coloring",      HourlyRate = 50m, IsAvailable = true };
            var staff2 = new Staff { UserId = staffUser2.Id, HairSalonId = salon1.Id, Specialization = "Hair Cut & Styling", HourlyRate = 45m, IsAvailable = true };
            var staff3 = new Staff { UserId = staffUser3.Id, HairSalonId = salon2.Id, Specialization = "Beard Grooming",     HourlyRate = 40m, IsAvailable = true };
            var staff4 = new Staff { UserId = staffUser4.Id, HairSalonId = salon2.Id, Specialization = "Nails & Extensions", HourlyRate = 35m, IsAvailable = true };
            var staff5 = new Staff { UserId = staffUser5.Id, HairSalonId = salon3.Id, Specialization = "Facial Treatments",  HourlyRate = 55m, IsAvailable = true };

            context.Staff.AddRange(staff1, staff2, staff3, staff4, staff5);
            context.SaveChanges();

            // ===== SERVICES =====
            var svc1 = new Service { HairSalonId = salon1.Id, Name = "Premium Hair Cut",        Description = "Professional haircut with styling",   Price = 60m,  DurationMinutes = 45,  Category = ServiceCategory.HairCut         };
            var svc2 = new Service { HairSalonId = salon1.Id, Name = "Hair Coloring",           Description = "Full head color treatment",           Price = 120m, DurationMinutes = 120, Category = ServiceCategory.Coloring        };
            var svc3 = new Service { HairSalonId = salon1.Id, Name = "Hair Treatment",          Description = "Deep conditioning treatment",         Price = 75m,  DurationMinutes = 60,  Category = ServiceCategory.Treatment       };
            var svc4 = new Service { HairSalonId = salon2.Id, Name = "Beard Trim",              Description = "Professional beard shaping and trim", Price = 35m,  DurationMinutes = 30,  Category = ServiceCategory.Beard           };
            var svc5 = new Service { HairSalonId = salon2.Id, Name = "Nail Manicure",           Description = "Full manicure with polish",           Price = 40m,  DurationMinutes = 45,  Category = ServiceCategory.Nails           };
            var svc6 = new Service { HairSalonId = salon2.Id, Name = "Styling",                 Description = "Professional hair styling",           Price = 50m,  DurationMinutes = 50,  Category = ServiceCategory.Styling         };
            var svc7 = new Service { HairSalonId = salon3.Id, Name = "Facial Treatment Deluxe", Description = "Premium facial with all treatments",  Price = 100m, DurationMinutes = 90,  Category = ServiceCategory.FacialTreatment };
            var svc8 = new Service { HairSalonId = salon3.Id, Name = "Hair Styling Pro",        Description = "Advanced styling techniques",         Price = 85m,  DurationMinutes = 75,  Category = ServiceCategory.Styling         };
            var svc9 = new Service { HairSalonId = salon3.Id, Name = "Hair Treatment Premium",  Description = "Luxury hair restoration",             Price = 150m, DurationMinutes = 90,  Category = ServiceCategory.Treatment       };

            context.Services.AddRange(svc1, svc2, svc3, svc4, svc5, svc6, svc7, svc8, svc9);
            context.SaveChanges();

            // ===== RESERVATIONS =====
            context.Reservations.AddRange(
                new Reservation { CustomerId = customer1.Id, StaffId = staff1.Id, ServiceId = svc1.Id, ReservationDateTime = new DateTime(2026, 4, 5,  10, 0,  0), Status = ReservationStatus.Confirmed, Notes = "First time customer, prefers fade cut", CreatedAt = DateTime.UtcNow },
                new Reservation { CustomerId = customer2.Id, StaffId = staff2.Id, ServiceId = svc2.Id, ReservationDateTime = new DateTime(2026, 4, 6,  14, 0,  0), Status = ReservationStatus.Confirmed, Notes = "Wants ash blonde color",               CreatedAt = DateTime.UtcNow },
                new Reservation { CustomerId = customer3.Id, StaffId = staff2.Id, ServiceId = svc3.Id, ReservationDateTime = new DateTime(2026, 4, 7,  11, 0,  0), Status = ReservationStatus.Pending,   Notes = "Dry hair needs treatment",             CreatedAt = DateTime.UtcNow },
                new Reservation { CustomerId = customer1.Id, StaffId = staff3.Id, ServiceId = svc4.Id, ReservationDateTime = new DateTime(2026, 4, 8,  9,  0,  0), Status = ReservationStatus.Completed, Notes = "Regular customer",                     CreatedAt = DateTime.UtcNow },
                new Reservation { CustomerId = customer2.Id, StaffId = staff4.Id, ServiceId = svc5.Id, ReservationDateTime = new DateTime(2026, 4, 9,  15, 30, 0), Status = ReservationStatus.Confirmed, Notes = "Gel manicure preferred",               CreatedAt = DateTime.UtcNow },
                new Reservation { CustomerId = customer3.Id, StaffId = staff4.Id, ServiceId = svc6.Id, ReservationDateTime = new DateTime(2026, 4, 10, 13, 0,  0), Status = ReservationStatus.Pending,   Notes = "Wedding event styling",                CreatedAt = DateTime.UtcNow },
                new Reservation { CustomerId = customer1.Id, StaffId = staff5.Id, ServiceId = svc7.Id, ReservationDateTime = new DateTime(2026, 4, 11, 10, 0,  0), Status = ReservationStatus.Confirmed, Notes = "Sensitive skin",                       CreatedAt = DateTime.UtcNow },
                new Reservation { CustomerId = customer2.Id, StaffId = staff5.Id, ServiceId = svc8.Id, ReservationDateTime = new DateTime(2026, 4, 12, 11, 0,  0), Status = ReservationStatus.Confirmed, Notes = "Special occasion",                     CreatedAt = DateTime.UtcNow },
                new Reservation { CustomerId = customer3.Id, StaffId = staff5.Id, ServiceId = svc9.Id, ReservationDateTime = new DateTime(2026, 4, 13, 14, 0,  0), Status = ReservationStatus.Cancelled, Notes = "Customer cancelled",                   CreatedAt = DateTime.UtcNow }
            );
            context.SaveChanges();
        }

        public static async Task SeedIdentityAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();

            string[] roles = { "Admin", "Staff", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            const string adminEmail = "admin@salon.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User"
                };
                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Repair: ensure every Customer-role AppUser has a matching BusinessUsers row
            var context = services.GetRequiredService<AppDbContext>();
            var customerRole = await roleManager.FindByNameAsync("Customer");
            if (customerRole != null)
            {
                var customerAppUserIds = context.UserRoles
                    .Where(ur => ur.RoleId == customerRole.Id)
                    .Select(ur => ur.UserId)
                    .ToHashSet();

                var appUsers = context.Users
                    .Where(u => customerAppUserIds.Contains(u.Id))
                    .ToList();

                foreach (var appUser in appUsers)
                {
                    if (!context.BusinessUsers.Any(u => u.Email == appUser.Email))
                    {
                        context.BusinessUsers.Add(new manage_my_hairsaloon.Models.User
                        {
                            Email = appUser.Email ?? string.Empty,
                            FirstName = appUser.FirstName,
                            LastName = appUser.LastName,
                            PasswordHash = "identity",
                            Role = manage_my_hairsaloon.Models.UserRole.Customer,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
                context.SaveChanges();
            }
        }
    }
}
