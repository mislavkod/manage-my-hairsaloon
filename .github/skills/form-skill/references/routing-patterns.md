# Routing Patterns Reference

## Default Convention Routing

Defined in `Program.cs`:
```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

This means:
- `GET /HairSalons/Index` → `HairSalonsController.Index()`
- `GET /HairSalons/Details/3` → `HairSalonsController.Details(3)`
- `GET /HairSalons/Create` → `HairSalonsController.Create()`
- `POST /HairSalons/Create` → `HairSalonsController.Create(model)` (POST action)
- `GET /HairSalons/Edit/3` → `HairSalonsController.Edit(3)`
- `POST /HairSalons/Edit/3` → `HairSalonsController.Edit(3, model)` (POST action)
- `GET /HairSalons/Delete/3` → `HairSalonsController.Delete(3)`
- `POST /HairSalons/Delete/3` → `HairSalonsController.DeleteConfirmed(3)` (with `[ActionName("Delete")]`)

For standard CRUD, do **not** add any route attributes — rely on convention.

## Attribute Routes for Custom Slugs

Use `[HttpGet("...")]` or `[HttpPost("...")]` when the desired URL doesn't match the convention.

```csharp
// Custom slug — different prefix than controller name
[HttpGet("salons/search/{name}")]
public IActionResult Search(string name) { ... }

// Salon-scoped nested resource
[HttpGet("salons/{salonId}/reservations/status/{status}")]
public IActionResult BySalonAndStatus(int salonId, ReservationStatus status) { ... }

// Availability filter
[HttpGet("Staff/Available")]
public IActionResult Available() { ... }
```

## Rules

| Scenario | Correct | Wrong |
|---|---|---|
| Custom slug | `[HttpGet("salons/search/{name}")]` | `[Route("salons/search/{name}")]` |
| POST action | `[HttpPost]` | `[Route]` + manual method check |
| CRUD actions | No attribute (convention) | `[HttpGet]` on every action |

**Why not `[Route]`?** — `[Route]` on an action doesn't specify an HTTP method, which conflicts with convention routing and can make endpoints unreachable.

## URL Casing

The project uses **PascalCase** for controller/action segments in convention routes:
- ✅ `/Staff/Available`
- ✅ `/HairSalons/Details/1`
- ❌ `/staff/available` (unless intentional custom slug)

Custom attribute routes may use lowercase slugs for RESTful style:
- `/salons/{salonId}/reservations/status/{status}` ← lowercase is fine here

## Tag Helpers vs Hard-Coded hrefs

For convention routes, always use tag helpers — they generate correct URLs automatically:
```html
<a asp-controller="HairSalons" asp-action="Edit" asp-route-id="@salon.Id">Edit</a>
<form asp-action="Create" method="post">
```

For custom attribute routes (slug-based), tag helpers cannot resolve them — use plain hrefs:
```html
<a href="/services/category/Beard">Beard</a>
<a href="/salons/@Model.Id/staff/available">Available Staff</a>
```
