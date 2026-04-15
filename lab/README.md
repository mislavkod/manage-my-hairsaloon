# Hair Salon Management System — Lab Documentation

This document covers everything built during this session: data models, repositories, controllers, and views.

---

## How to Run

```
dotnet run
```

Navigate to `https://localhost:{port}` (shown in terminal on startup). The app opens on the Home dashboard.

---

## Data Models

The system has 6 entity classes and 2 enums, all in `Models/`.

### User
Represents all users — customers, staff, and admins.

```csharp
public int Id { get; set; }
public string? Email { get; set; }
public string? PasswordHash { get; set; }
public string? FirstName { get; set; }
public string? LastName { get; set; }
public string? PhoneNumber { get; set; }
public UserRole Role { get; set; }          // Enum: Customer, Staff, Admin
public DateTime CreatedAt { get; set; }

// Navigation properties
public Staff? Staff { get; set; }                          // 1-to-1 if user is staff
public List<Reservation> CustomerReservations { get; set; } // reservations they made
public List<Review> Reviews { get; set; }                   // reviews they wrote
```

### UserRole (Enum)
```csharp
Customer   // Can book reservations
Staff      // Works at a salon
Admin      // Manages the system
```

### HairSalon
Represents a physical salon location.

```csharp
public int Id { get; set; }
public string? Name { get; set; }
public string? Address { get; set; }
public string? PhoneNumber { get; set; }
public string? Email { get; set; }
public DateTime CreatedAt { get; set; }

// Navigation properties
public List<Staff> Staff { get; set; }       // all staff at this salon
public List<Service> Services { get; set; }  // all services offered here
```

### Staff
Links a User to a HairSalon as an employee.

```csharp
public int Id { get; set; }
public int UserId { get; set; }       // FK → User
public int HairSalonId { get; set; }  // FK → HairSalon
public string? Specialization { get; set; }
public decimal HourlyRate { get; set; }
public bool IsAvailable { get; set; }

// Navigation properties
public User? User { get; set; }
public HairSalon? HairSalon { get; set; }
public List<Reservation> Reservations { get; set; }  // appointments assigned to them
```

### Service
A service offered by a salon (haircut, coloring, etc.).

```csharp
public int Id { get; set; }
public int HairSalonId { get; set; }  // FK → HairSalon
public string? Name { get; set; }
public string? Description { get; set; }
public decimal Price { get; set; }
public int DurationMinutes { get; set; }
public ServiceCategory Category { get; set; }  // Enum

// Navigation properties
public HairSalon? HairSalon { get; set; }
public List<Reservation> Reservations { get; set; }
```

### ServiceCategory (Enum)
```csharp
HairCut | Coloring | Styling | Treatment | Beard | Nails | FacialTreatment
```

### Reservation
A booking by a customer for a specific service with a specific staff member.

```csharp
public int Id { get; set; }
public int CustomerId { get; set; }  // FK → User
public int StaffId { get; set; }     // FK → Staff
public int ServiceId { get; set; }   // FK → Service
public DateTime ReservationDateTime { get; set; }
public ReservationStatus Status { get; set; }  // Enum
public string? Notes { get; set; }
public DateTime CreatedAt { get; set; }

// Navigation properties
public User? Customer { get; set; }
public Staff? Staff { get; set; }
public Service? Service { get; set; }
public List<Review> Reviews { get; set; }
```

### ReservationStatus (Enum)
```csharp
Pending     // Waiting for confirmation
Confirmed   // Appointment confirmed
Completed   // Service delivered
Cancelled   // Customer cancelled
NoShow      // Customer didn't show up
```

### Review
A customer's rating and comment for a completed reservation.

```csharp
public int Id { get; set; }
public int ReservationId { get; set; }  // FK → Reservation
public int CustomerId { get; set; }     // FK → User
public int Rating { get; set; }         // 1–5
public string? Comment { get; set; }
public DateTime CreatedAt { get; set; }

// Navigation properties
public Reservation? Reservation { get; set; }
public User? Customer { get; set; }
```

---

## Entity Relationships

```
HairSalon (1) ──── (many) Staff ──── (many) Reservations ──── (many) Reviews
HairSalon (1) ──── (many) Services ─────────────┘
User (1) ──────────────────────────── (many) Reservations (as Customer)
User (1) ──────── (1) Staff
```

| From | To | Type | Meaning |
|---|---|---|---|
| HairSalon | Staff | 1-to-many | A salon employs many staff |
| HairSalon | Service | 1-to-many | A salon offers many services |
| User | Staff | 1-to-1 | A staff-role user has one staff profile |
| User | Reservation | 1-to-many | A customer can have many reservations |
| User | Review | 1-to-many | A customer can write many reviews |
| Staff | Reservation | 1-to-many | A staff member handles many reservations |
| Service | Reservation | 1-to-many | A service can be booked many times |
| Reservation | Review | 1-to-many | A reservation can receive many reviews |

### How reservations relate back to a salon

There is no direct `HairSalonId` on `Reservation`. The path is:

```
HairSalon → Staff[] → Reservations[]
```

In Razor views this is written with `SelectMany`:

```csharp
var salonReservations = Model.Staff
    .SelectMany(s => s.Reservations)
    .ToList();
```

---

## Repositories

All repositories live in `Repositories/` and read from the static `MockDataStore`. They are **not** connected to a database — all data is hardcoded in memory.

| Repository | Methods |
|---|---|
| `MockUserRepository` | `GetAll()`, `GetById(id)`, `GetByRole(role)`, `GetCustomers()`, `GetStaffUsers()`, `GetByEmail(email)` |
| `MockHairSalonRepository` | `GetAll()`, `GetById(id)`, `GetByName(name)` |
| `MockStaffRepository` | `GetAll()`, `GetById(id)`, `GetBySalonId(id)`, `GetByUserId(id)`, `GetAvailable()` |
| `MockServiceRepository` | `GetAll()`, `GetById(id)`, `GetBySalonId(id)`, `GetByCategory(cat)`, `GetByMaxPrice(price)` |
| `MockReservationRepository` | `GetAll()`, `GetById(id)`, `GetByCustomerId(id)`, `GetByStaffId(id)`, `GetByStatus(status)`, `GetByServiceId(id)` |
| `MockReviewRepository` | `GetAll()`, `GetById(id)`, `GetByReservationId(id)`, `GetByCustomerId(id)` |

### MockDataStore

`Repositories/MockDataStore.cs` is a static class whose constructor builds all in-memory data: 3 salons, 5 staff members (linked to user accounts), 9 services, 9 reservations, 0 reviews (empty list). All navigation properties are wired up manually (e.g. `salon1.Staff.Add(staffSalon1_1)`).

### Dependency injection

Repositories are registered as singletons in `Program.cs`:

```csharp
builder.Services.AddSingleton<MockUserRepository>();
builder.Services.AddSingleton<MockHairSalonRepository>();
builder.Services.AddSingleton<MockStaffRepository>();
builder.Services.AddSingleton<MockServiceRepository>();
builder.Services.AddSingleton<MockReservationRepository>();
builder.Services.AddSingleton<MockReviewRepository>();
```

They are injected into controllers via constructor injection:

```csharp
public class UsersController : Controller
{
    private readonly MockUserRepository _userRepo;

    public UsersController(MockUserRepository userRepo)
    {
        _userRepo = userRepo;
    }
}
```

---

## Controllers and Views

Each entity has its own controller with `Index` (list) and `Details` (single item) actions.

| Controller | Route | What it shows |
|---|---|---|
| `HomeController` | `/` | Dashboard with counts |
| `UsersController` | `/Users` | All users table + detail page |
| `HairSalonsController` | `/HairSalons` | All salons + detail (with staff, services, reservations, reviews) |
| `StaffController` | `/Staff` | All staff + detail (with reservations) |
| `ServicesController` | `/Services` | All services + detail (with reservations) |
| `ReservationsController` | `/Reservations` | All reservations + detail |
| `ReviewsController` | `/Reviews` | All reviews + detail |

### How data flows from controller to view

1. Controller calls the repository: `var users = _userRepo.GetAll();`
2. Passes the model to the view: `return View(users);`
3. View declares the model type at the top: `@model List<manage_my_hairsaloon.Models.User>`
4. View iterates or displays the data with Razor syntax: `@user.FirstName`

Example for a detail page:

```csharp
// Controller
public IActionResult Details(int id)
{
    var salon = _salonRepo.GetById(id);
    if (salon == null) return NotFound();
    return View(salon);       // passes HairSalon object to view
}
```

```html
<!-- View -->
@model manage_my_hairsaloon.Models.HairSalon

<h1>@Model.Name</h1>
<p>@Model.Address</p>

@foreach (var staff in Model.Staff)
{
    <p>@staff.User?.FirstName — @staff.Specialization</p>
}
```

### Views folder structure

```
Views/
  Home/           Index.cshtml (dashboard)
  Users/          Index.cshtml, Details.cshtml
  HairSalons/     Index.cshtml, Details.cshtml
  Staff/          Index.cshtml, Details.cshtml
  Services/       Index.cshtml, Details.cshtml
  Reservations/   Index.cshtml, Details.cshtml
  Reviews/        Index.cshtml, Details.cshtml
  Shared/         _Layout.cshtml (nav + page shell)
```

### Back navigation (returnUrl pattern)

When navigating from a detail page to a child detail page, a `returnUrl` query parameter is passed:

```html
<a asp-controller="Staff" asp-action="Details"
   asp-route-id="@staff.Id"
   asp-route-returnUrl="@Context.Request.PathBase@Context.Request.Path">
   Details
</a>
```

The target detail page reads it and uses it for the back link:

```csharp
var returnUrl = Context.Request.Query["returnUrl"].FirstOrDefault();
```

```html
@if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
{
    <a href="@returnUrl">← Back</a>
}
else
{
    <a asp-controller="Staff" asp-action="Index">← Back to Staff</a>
}
```

`Url.IsLocalUrl()` validates the URL to prevent open-redirect attacks.

---

## UI / Styling

All styling lives in `wwwroot/css/site.css`. The design follows a **60s/70s high-end barbershop** aesthetic:

| Role | Color |
|---|---|
| Primary background | Rich Mahogany `#4E1A0E` |
| Secondary background | Toasted Oak `#7B4F2E` |
| Accent / highlight | Brass Gold `#C9A84C` |
| Warm glow | Amber Glass `#D97B2B` |
| Text on dark | Cream Parchment `#F5ECD7` |

The background has a repeating SVG pattern of barbershop icons (scissors, razor, mustache, comb, pole) in brass gold at very low opacity for texture.

Google Fonts used: `Playfair Display` (headings) and `Raleway` (body).

---

## Mock Data Summary

| Entity | Count |
|---|---|
| Users | 8 (3 customers, 5 staff) |
| HairSalons | 3 |
| Staff | 5 |
| Services | 9 |
| Reservations | 9 |
| Reviews | 0 |

### Salons and staff

| Salon | Staff |
|---|---|
| Premium Cuts Downtown | Anna Wilson (Hair Coloring), Bob Brown (Hair Cut & Styling) |
| Beauty Oasis Mall | Sarah Davis (Beard Grooming), Tom Miller (Nails & Extensions) |
| Elegant Styles Uptown | Lucy Taylor (Facial Treatments) |
