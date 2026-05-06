---
name: form-skill
description: "Use when: creating web forms for the manage-my-hairsaloon ASP.NET Core MVC app. Covers GET filter forms and POST create/edit/delete forms. Handles wiring controller actions, repository calls, ModelState validation (server-side), and Razor view markup. Use for any task that involves collecting input from the user and persisting it to the database or filtering data."
argument-hint: "Describe the form to create, e.g. 'create reservation form' or 'edit service form'"
---

# Form Skill — manage-my-hairsaloon

Handles end-to-end form creation for this ASP.NET Core MVC app: Razor view markup, controller actions (GET + POST), repository layer calls, and server-side ModelState validation.

## Project Conventions

| Convention | Value |
|---|---|
| Namespace | `manage_my_hairsaloon` |
| ORM | Entity Framework Core 8 via `AppDbContext` |
| Data access | Repository pattern — always go through the interface, never inject `AppDbContext` into controllers |
| Route convention | `{Controller}/{Action}/{id?}` (default) — use `[HttpGet("...")]` for custom slug routes |
| Validation | Server-side only: Data Annotations on models + `ModelState.IsValid` in POST actions |
| View location | `Views/{ControllerName}/{ActionName}.cshtml` |
| Shared layout | `Views/Shared/_Layout.cshtml` — all views inherit it via `_ViewStart.cshtml` |
| CSS classes | `btn-outline-brass`, `input-barbershop`, `table-barbershop`, `badge-brass`, `card-barbershop`, `filter-bar` |

## When to Use This Skill

- Creating a **Create** form (`GET` renders empty form, `POST` saves to DB)
- Creating an **Edit** form (`GET` loads existing record, `POST` updates DB)
- Adding a **Delete** confirmation + action
- Adding a **filter/search** form (GET only, calls read-only repository methods)
- Wiring a new form action to an existing or new repository method

---

## Step-by-Step: POST Form (Create / Edit / Delete)

### Step 1 — Add Data Annotations to the Model

Add validation attributes to the model class. Models are in `Models/`.

```csharp
using System.ComponentModel.DataAnnotations;

public class HairSalon
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100)]
    public string? Name { get; set; }

    [Required]
    public string? Address { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public DateTime CreatedAt { get; set; }
    // navigation properties stay as-is
}
```

> Only annotate scalar properties. Never annotate navigation properties (`List<Staff>`, etc.).

### Step 2 — Add Repository Write Methods

Check [repository-api.md](./references/repository-api.md) to see what already exists.  
If the needed method (e.g. `Add`, `Update`, `Delete`) is missing:

1. Add the method signature to the **interface** in `Repositories/I{Entity}Repository.cs`
2. Implement it in `Repositories/{Entity}Repository.cs` using `AppDbContext`

**Standard write method patterns:**

```csharp
// Interface
void Add(HairSalon salon);
void Update(HairSalon salon);
void Delete(int id);

// Implementation (inject AppDbContext via constructor)
public void Add(HairSalon salon)
{
    _context.HairSalons.Add(salon);
    _context.SaveChanges();
}

public void Update(HairSalon salon)
{
    _context.HairSalons.Update(salon);
    _context.SaveChanges();
}

public void Delete(int id)
{
    var salon = _context.HairSalons.Find(id);
    if (salon != null)
    {
        _context.HairSalons.Remove(salon);
        _context.SaveChanges();
    }
}
```

> `SaveChanges()` must be called explicitly — EF does not auto-save.

### Step 3 — Add Controller Actions

See [controller-patterns.md](./references/controller-patterns.md) for the full patterns.

**Create (GET + POST):**
```csharp
// GET: /HairSalons/Create
public IActionResult Create()
{
    // Populate ViewBag dropdowns here if needed (see Step 3a)
    return View();
}

// POST: /HairSalons/Create
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Create(HairSalon model)
{
    if (!ModelState.IsValid)
        return View(model);   // re-render form with validation errors

    model.CreatedAt = DateTime.UtcNow;
    _salonRepo.Add(model);
    return RedirectToAction(nameof(Index));
}
```

**Edit (GET + POST):**
```csharp
// GET: /HairSalons/Edit/5
public IActionResult Edit(int id)
{
    var salon = _salonRepo.GetById(id);
    if (salon == null) return NotFound();
    return View(salon);
}

// POST: /HairSalons/Edit/5
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Edit(int id, HairSalon model)
{
    if (id != model.Id) return BadRequest();
    if (!ModelState.IsValid)
        return View(model);

    _salonRepo.Update(model);
    return RedirectToAction(nameof(Index));
}
```

**Delete (GET confirmation + POST):**
```csharp
// GET: /HairSalons/Delete/5
public IActionResult Delete(int id)
{
    var salon = _salonRepo.GetById(id);
    if (salon == null) return NotFound();
    return View(salon);
}

// POST: /HairSalons/Delete/5
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public IActionResult DeleteConfirmed(int id)
{
    _salonRepo.Delete(id);
    return RedirectToAction(nameof(Index));
}
```

#### Step 3a — Dropdown Lists (SelectLists for FK fields)

When a form field is a foreign key (e.g. `HairSalonId` on a `Service`), pass the list via `ViewBag`:

```csharp
// In GET action, before return View():
ViewBag.Salons = new SelectList(_salonRepo.GetAll(), "Id", "Name");
// In POST action, if ModelState.IsValid fails, repopulate before return View(model):
ViewBag.Salons = new SelectList(_salonRepo.GetAll(), "Id", "Name", model.HairSalonId);
```

For enum fields (`ServiceCategory`, `ReservationStatus`), use:
```csharp
ViewBag.Categories = new SelectList(Enum.GetValues<ServiceCategory>());
```

### Step 4 — Create the Razor View

**Create / Edit form template:**
```html
@model manage_my_hairsaloon.Models.HairSalon
@{
    ViewData["Title"] = "Create Salon";
}

<div class="page-header">
    <h1>Create Salon</h1>
</div>

<div class="card-barbershop" style="max-width:560px;">
    <form asp-action="Create" method="post">
        @Html.AntiForgeryToken()

        <div asp-validation-summary="ModelOnly" class="text-danger" style="margin-bottom:1rem;"></div>

        <div class="form-group" style="margin-bottom:1rem;">
            <label asp-for="Name"></label>
            <input asp-for="Name" class="input-barbershop" />
            <span asp-validation-for="Name" class="text-danger" style="font-size:0.8rem;"></span>
        </div>

        <div class="form-group" style="margin-bottom:1rem;">
            <label asp-for="Address"></label>
            <input asp-for="Address" class="input-barbershop" />
            <span asp-validation-for="Address" class="text-danger" style="font-size:0.8rem;"></span>
        </div>

        <!-- FK dropdown example -->
        @* <div class="form-group" style="margin-bottom:1rem;">
            <label asp-for="HairSalonId">Salon</label>
            <select asp-for="HairSalonId" asp-items="ViewBag.Salons" class="input-barbershop">
                <option value="">-- Select --</option>
            </select>
            <span asp-validation-for="HairSalonId" class="text-danger" style="font-size:0.8rem;"></span>
        </div> *@

        <div style="display:flex; gap:0.75rem; margin-top:1.5rem;">
            <button type="submit" class="btn-outline-brass">Save</button>
            <a asp-action="Index" class="back-link" style="align-self:center;">Cancel</a>
        </div>
    </form>
</div>
```

**Delete confirmation view template:**
```html
@model manage_my_hairsaloon.Models.HairSalon
@{
    ViewData["Title"] = "Delete Salon";
}

<div class="page-header">
    <h1>Delete Salon</h1>
</div>

<div class="card-barbershop" style="max-width:480px;">
    <p>Are you sure you want to delete <strong>@Model.Name</strong>?</p>
    <form asp-action="Delete" method="post">
        @Html.AntiForgeryToken()
        <input type="hidden" asp-for="Id" />
        <div style="display:flex; gap:0.75rem; margin-top:1rem;">
            <button type="submit" class="btn-outline-brass" style="background:var(--clr-mahogany);">Delete</button>
            <a asp-action="Index" class="back-link" style="align-self:center;">Cancel</a>
        </div>
    </form>
</div>
```

---

## Step-by-Step: GET Filter Form

GET filter forms call existing read-only repository methods and re-render an Index-style view.

### Controller action pattern

```csharp
// Attribute route (custom slug) — use [HttpGet] not [Route]
[HttpGet("services/category/{category}")]
public IActionResult ByCategory(ServiceCategory category)
{
    var services = _serviceRepo.GetByCategory(category);
    return View("Index", services);  // reuse existing Index view
}
```

For query-string-based filters (no slug):
```csharp
// Convention route: /Services/Filter?maxPrice=50
public IActionResult Filter(decimal? maxPrice)
{
    var services = maxPrice.HasValue
        ? _serviceRepo.GetByMaxPrice(maxPrice.Value)
        : _serviceRepo.GetAll();
    return View("Index", services);
}
```

### View: add filter controls above the table

Use plain `<a href="...">` links for slug-based filters (tag helpers can't generate custom attribute routes):
```html
<div class="filter-bar" style="display:flex; gap:0.5rem; flex-wrap:wrap; margin-bottom:1rem;">
    <a href="/Services/Index" class="btn-outline-brass">All</a>
    <a href="/services/category/HairCut" class="btn-outline-brass">Haircut</a>
    <a href="/services/category/Beard" class="btn-outline-brass">Beard</a>
</div>
```

For query-string filters, use a standard `<form method="get">`:
```html
<form method="get" asp-action="Filter" style="display:flex; gap:0.5rem; margin-bottom:1rem;">
    <input type="number" name="maxPrice" class="input-barbershop" placeholder="Max price..." />
    <button type="submit" class="btn-outline-brass">Filter</button>
</form>
```

---

## FK Relationship Rules

When creating records that have FK fields, ensure the IDs come from existing data — never hard-code IDs in forms. Always use `SelectList` as shown in Step 3a.

| FK field | Source repository method |
|---|---|
| `Service.HairSalonId` | `IHairSalonRepository.GetAll()` |
| `Staff.HairSalonId` | `IHairSalonRepository.GetAll()` |
| `Staff.UserId` | `IUserRepository.GetAll()` |
| `Reservation.CustomerId` | `IUserRepository.GetAll()` |
| `Reservation.StaffId` | `IStaffRepository.GetAll()` |
| `Reservation.ServiceId` | `IServiceRepository.GetAll()` |
| `Review.ReservationId` | `IReservationRepository.GetAll()` |

---

## Routing Quick Reference

See [routing-patterns.md](./references/routing-patterns.md) for full details.

| Use case | Route attribute | URL produced |
|---|---|---|
| Standard CRUD | None (convention) | `/HairSalons/Create`, `/HairSalons/Edit/5` |
| Custom slug | `[HttpGet("salons/search/{name}")]` | `/salons/search/Cuts` |
| Salon-scoped | `[HttpGet("salons/{salonId}/staff/available")]` | `/salons/1/staff/available` |

**Never use `[Route(...)]` on individual actions** — it removes the HTTP method restriction and conflicts with convention routing. Always use `[HttpGet]` or `[HttpPost]`.

---

## Available Repository Methods

See [repository-api.md](./references/repository-api.md) for the full list of existing read and write methods per entity.

---

## Checklist Before Finishing

- [ ] Data Annotations added to model (scalar properties only)
- [ ] Repository interface has the needed write methods
- [ ] Repository implementation calls `SaveChanges()`
- [ ] POST action has `[ValidateAntiForgeryToken]`
- [ ] `ModelState.IsValid` checked before writing to DB
- [ ] ViewBag repopulated in POST action when returning `View(model)`
- [ ] View has `@Html.AntiForgeryToken()` inside the `<form>`
- [ ] View has `asp-validation-summary` and per-field `asp-validation-for` spans
- [ ] `asp-for` tag helpers used on all inputs (not manual `name=` attributes)
- [ ] After successful POST, controller returns `RedirectToAction` (PRG pattern)
