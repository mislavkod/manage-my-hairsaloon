using manage_my_hairsaloon.Models;

namespace manage_my_hairsaloon.Repositories
{
    public static class MockDataStore
    {
        public static List<User> Users { get; }
        public static List<HairSalon> HairSalons { get; }
        public static List<Staff> Staff { get; }
        public static List<Service> Services { get; }
        public static List<Reservation> Reservations { get; }
        public static List<Review> Reviews { get; }

        static MockDataStore()
        {
            // ===== USERS =====
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

            // ===== HAIR SALONS =====
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

            // ===== STAFF =====
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

            // ===== SERVICES =====
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

            // ===== RESERVATIONS =====
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

            // ===== ASSIGN STATIC PROPERTIES =====
            Users = new List<User> { customer1, customer2, customer3, staff1, staff2, staff3, staff4, staff5 };
            HairSalons = new List<HairSalon> { salon1, salon2, salon3 };
            Staff = new List<Staff> { staffSalon1_1, staffSalon1_2, staffSalon2_1, staffSalon2_2, staffSalon3_1 };
            Services = new List<Service> { service1, service2, service3, service4, service5, service6, service7, service8, service9 };
            Reservations = new List<Reservation> { reservation1, reservation2, reservation3, reservation4, reservation5, reservation6, reservation7, reservation8, reservation9 };
            Reviews = new List<Review>();
        }
    }
}
