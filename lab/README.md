# Hair Salon Management System — Lab Documentation

This document covers everything built across all lab sessions: data models, EF Core + Docker setup, repository pattern, controllers, custom attribute routing, UI filter controls, and the reservation form.

---

## How to Run

### Prerequisites
- Docker Desktop running
- .NET 8 SDK

### 1 — Start the database
```bash
docker compose up -d
```
This starts MSSQL Server 2022 on port 1433. Data persists in the `mssql_data` Docker volume.

### 2 — Run the app
```bash
dotnet run
```
On first run, EF applies migrations and seeds all data automatically. Navigate to `https://localhost:{port}` shown in the terminal.

### Stopping
```bash
docker compose down   # stops container (data kept in volume)
```

---

## Database Setup (Entity Framework Core + Docker)

### docker-compose.yml
Defines the MSSQL Server container:
```yaml
services:
  mssql:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      SA_PASSWORD: "Projekt123!"
      ACCEPT_EULA: "Y"
    ports:
      - "1433:1433"
    volumes:
      - mssql_data:/var/opt/mssql
volumes:
  mssql_data:
```

### Connection string (appsettings.Development.json)
```json
"DefaultConnection": "Server=localhost,1433;Database=HairSalonDb;User Id=sa;Password=Projekt123!;TrustServerCertificate=True"
```
`appsettings.json` contains a placeholder `CHANGE_ME` password — the real password only lives in `Development`.

### NuGet packages (all pinned to 8.0.7 to match net8.0 target)
```
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
Microsoft.EntityFrameworkCore.Design
```

### AppDbContext (`Data/AppDbContext.cs`)
Bridges C# models to the database. Key configuration in `OnModelCreating`:
- `decimal(10,2)` precision for `Service.Price` and `Staff.HourlyRate`
- 1-to-1 between `User` and `Staff`
- All `Reservation` FK relationships use `DeleteBehavior.Restrict` to avoid cascade conflicts
- `Review → Reservation` uses `DeleteBehavior.Cascade`

### Migrations
Two migrations applied:
1. `InitialCreate` — creates all 6 tables
2. `FixDecimalPrecision` — alters Price and HourlyRate columns to `decimal(10,2)`

Run migrations manually with:
```bash
dotnet ef migrations add <Name>
dotnet ef database update
```

### Database seeding (`Data/DbSeeder.cs`)
Called from `Program.cs` on every startup. Has an early-exit guard (`if (context.Users.Any()) return;`) so it only seeds once. Seeds in dependency order (Users → Salons → Staff → Services → Reservations) and calls `SaveChanges()` after each group so SQL Server IDENTITY-assigned IDs are available for FK references.

---



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

All repositories live in `Repositories/`. Each entity has an **interface** and an **EF Core implementation**. Controllers always inject the interface — never the concrete class or `AppDbContext` directly.

> The original `Mock*` files still exist in the folder but are no longer registered or used.

### Interfaces and methods

| Interface | Read methods | Write methods |
|---|---|---|
| `IUserRepository` | `GetAll()`, `GetById()`, `GetByRole()`, `GetCustomers()`, `GetStaffUsers()`, `GetByEmail()` | — |
| `IHairSalonRepository` | `GetAll()`, `GetById()`, `GetByName()` | — |
| `IStaffRepository` | `GetAll()`, `GetById()`, `GetBySalonId()`, `GetByUserId()`, `GetAvailable()`, `GetAvailableBySalonId()` | — |
| `IServiceRepository` | `GetAll()`, `GetById()`, `GetBySalonId()`, `GetByCategory()`, `GetByMaxPrice()` | — |
| `IReservationRepository` | `GetAll()`, `GetById()`, `GetByCustomerId()`, `GetByStaffId()`, `GetByStatus()`, `GetByServiceId()`, `GetBySalonIdAndStatus()`, `GetBySalonIdAndDate()` | `Add()` |
| `IReviewRepository` | `GetAll()`, `GetById()` | — |

All implementations use `.Include()` for navigation properties. `ReservationRepository.Add()` calls `SaveChanges()` internally.

### Dependency injection (Program.cs)
Registered as `Scoped` (one instance per HTTP request):
```csharp
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IHairSalonRepository, HairSalonRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
```

### Constructor injection in controllers
```csharp
public class ServicesController : Controller
{
    private readonly IServiceRepository _serviceRepo;

    public ServicesController(IServiceRepository serviceRepo)
    {
        _serviceRepo = serviceRepo;
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
  HairSalons/     Index.cshtml, Details.cshtml, NewReservations.cshtml
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

## Custom Attribute Routing

In addition to the default `{Controller}/{Action}/{id?}` convention, the following custom attribute routes exist. All use `[HttpGet("...")]` — **never `[Route]`** — to avoid conflicts with convention routing.

| URL | Controller action | Description |
|---|---|---|
| `/services/category/{category}` | `ServicesController.ByCategory` | Services filtered by `ServiceCategory` enum |
| `/Staff/Available` | `StaffController.Available` | All staff where `IsAvailable == true` |
| `/Staff/Available` (salon-scoped) | `StaffController.AvailableBySalon` | — see below |
| `/reservations/status/{status}` | `ReservationsController.ByStatus` | Reservations filtered by `ReservationStatus` enum |
| `/salons/search/{name}` | `HairSalonsController.Search` | Salon found by exact name |
| `/salons/{salonId}/staff/available` | `StaffController.AvailableBySalon` | Available staff for a specific salon |
| `/salons/{salonId}/reservations/status/{status}` | `ReservationsController.BySalonAndStatus` | Reservations for a salon + status combo |
| `/salons/{salonId}/NewReservations` | `HairSalonsController.NewReservation` (GET + POST) | Reservation booking form |

### Why `[HttpGet]` not `[Route]`

`[Route]` on an individual action doesn't restrict the HTTP method, which causes it to conflict with the default convention routing and makes endpoints unreachable. `[HttpGet("...")]` specifies both path and method.

```csharp
// Correct
[HttpGet("services/category/{category}")]
public IActionResult ByCategory(ServiceCategory category) { ... }

// Wrong — can make the action unreachable
[Route("services/category/{category}")]
public IActionResult ByCategory(ServiceCategory category) { ... }
```

### Enum binding in routes

ASP.NET Core automatically binds enum names in URL segments. The enum name must match exactly (case-insensitive):

```
/services/category/HairCut    → ServiceCategory.HairCut  ✓
/reservations/status/Confirmed → ReservationStatus.Confirmed  ✓
```

---

## UI Filter Controls

Each index page has filter controls so users don't need to type URLs manually.

| Page | Controls |
|---|---|
| `Services/Index` | Category filter pill buttons (All + one per `ServiceCategory`) |
| `Staff/Index` | "All Staff" / "Available Only" buttons |
| `Reservations/Index` | Status filter pill buttons (All + one per `ReservationStatus`) |
| `HairSalons/Index` | Live text filter (JS `oninput`) — filters table rows by salon name client-side |
| `HairSalons/Details` | "Available Staff Only" link + per-status reservation filter buttons scoped to that salon |

Filter buttons that target custom attribute routes use plain `<a href="...">` links (tag helpers cannot generate custom attribute route URLs):
```html
<a href="/services/category/HairCut" class="btn-outline-brass">Haircut</a>
```

Salon-scoped filter buttons use the model ID:
```html
<a href="/salons/@Model.Id/staff/available" class="btn-outline-brass">Available Staff Only</a>
```

---

## New Reservation Form

Accessible from each salon's detail page via the **"Make a Reservation"** button.

### URL
- `GET  /salons/{salonId}/NewReservations` — renders the form (date step)
- `GET  /salons/{salonId}/NewReservations?date=2026-05-10` — shows available time slots for that date
- `POST /salons/{salonId}/NewReservations` — saves the reservation

### Two-step flow

**Step 1 — Pick a date**  
A GET form submits `?date=YYYY-MM-DD`. The controller calls `GetBySalonIdAndDate` to find reservations that day, then computes which 1-hour slots (09:00–18:00) still have a free staff member.

**Step 2 — Complete the form**  
Only shown when `AvailableHours` is non-empty. The user selects:
- A time slot (radio buttons styled as brass pill buttons)
- A service (dropdown: Name — price, duration)
- A staff member (dropdown: First Last)
- A customer (dropdown — temporary until authentication is added)
- Optional notes

### Time-slot availability logic
A slot is available when the number of staff already booked at that hour is less than the total number of available staff at the salon:
```csharp
vm.AvailableHours = WorkHours
    .Where(h => !bookedByHour.ContainsKey(h) || bookedByHour[h] < staffIds.Count)
    .ToList();
```
Cancelled reservations are excluded from the booked count.

### ViewModel (`ViewModels/NewReservationViewModel.cs`)
Combines display data (populated by controller) and form input (validated on POST):
```csharp
// Display data
HairSalon Salon
List<Service> Services
List<Staff> AvailableStaff
List<User> Customers
List<int> AvailableHours

// Form input (validated)
int ServiceId        [Required]
int StaffId          [Required]
int CustomerId       [Required]
DateTime? SelectedDate  [Required]
int? SelectedHour    [Required, Range(9,18)]
string? Notes        [StringLength(500)]
```

### Backend wiring
- `IReservationRepository.GetBySalonIdAndDate(salonId, date)` — new method added
- `IReservationRepository.Add(reservation)` — new method added
- `HairSalonsController` now injects 5 repositories: `IHairSalonRepository`, `IServiceRepository`, `IStaffRepository`, `IReservationRepository`, `IUserRepository`

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

### CSS utility classes

| Class | Purpose |
|---|---|
| `btn-outline-brass` | Transparent button with brass border; fills on hover |
| `input-barbershop` | Styled `<input>`, `<select>`, `<textarea>` — mahogany background, brass border, cream text, amber focus glow |
| `table-barbershop` | Dark table with brass header accents |
| `card-barbershop` | Content card with mahogany background + brass border |
| `badge-brass` | Small brass pill label |
| `badge-available` / `badge-unavailable` | Staff availability indicators |
| `badge-status-*` | Reservation status colour-coded badges |
| `slot-pill` | Time-slot radio buttons on reservation form (brass outline; fills on selection) |
| `filter-bar` | Flex wrapper for filter button rows |
| `page-header` | Top `<h1>` section with decorative rule |
| `section-title` | In-page `<h2>` headings |
| `divider-ornament` | Centred `— ◆ —` decorative divider |
| `back-link` | Styled `← Back` link |

---

## Seeded Data

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

---

## Copilot Customizations

### form-skill (`.github/skills/form-skill/`)
A Copilot agent skill for creating web forms in this app. Invoke it with `/form-skill` or let the agent auto-discover it when asked to create forms.

Covers:
- Step-by-step procedure for POST forms (Create / Edit / Delete) with PRG pattern
- Step-by-step procedure for GET filter forms (slug-based and query-string)
- Server-side ModelState validation and Data Annotations
- FK dropdown population via `ViewBag` + `SelectList`
- Routing rules (`[HttpGet]` vs `[Route]` vs convention)

Reference files bundled with the skill:
| File | Contents |
|---|---|
| `references/repository-api.md` | All existing interface methods + write method patterns |
| `references/routing-patterns.md` | Convention vs attribute routes, tag helper vs plain href rules |
| `references/controller-patterns.md` | Full CRUD action set, dropdown helper, anti-forgery, ModelState removal |
