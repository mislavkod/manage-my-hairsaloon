# Controller Patterns Reference

## Constructor Injection

Each controller injects only the repository interfaces it needs:

```csharp
public class HairSalonsController : Controller
{
    private readonly IHairSalonRepository _salonRepo;

    public HairSalonsController(IHairSalonRepository salonRepo)
    {
        _salonRepo = salonRepo;
    }
}
```

When a form needs related data (e.g. a dropdown of salons in a Service form), inject multiple repositories:

```csharp
public class ServicesController : Controller
{
    private readonly IServiceRepository _serviceRepo;
    private readonly IHairSalonRepository _salonRepo;

    public ServicesController(IServiceRepository serviceRepo, IHairSalonRepository salonRepo)
    {
        _serviceRepo = serviceRepo;
        _salonRepo = salonRepo;
    }
}
```

## Full CRUD Action Set

```csharp
// GET /Entity/Index
public IActionResult Index()
{
    return View(_repo.GetAll());
}

// GET /Entity/Details/5
public IActionResult Details(int id)
{
    var item = _repo.GetById(id);
    if (item == null) return NotFound();
    return View(item);
}

// GET /Entity/Create
public IActionResult Create()
{
    PopulateDropdowns();   // optional, see below
    return View();
}

// POST /Entity/Create
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Create(Entity model)
{
    if (!ModelState.IsValid)
    {
        PopulateDropdowns(model);
        return View(model);
    }
    model.CreatedAt = DateTime.UtcNow;  // set server-controlled fields here
    _repo.Add(model);
    return RedirectToAction(nameof(Index));
}

// GET /Entity/Edit/5
public IActionResult Edit(int id)
{
    var item = _repo.GetById(id);
    if (item == null) return NotFound();
    PopulateDropdowns(item);
    return View(item);
}

// POST /Entity/Edit/5
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Edit(int id, Entity model)
{
    if (id != model.Id) return BadRequest();
    if (!ModelState.IsValid)
    {
        PopulateDropdowns(model);
        return View(model);
    }
    _repo.Update(model);
    return RedirectToAction(nameof(Index));
}

// GET /Entity/Delete/5
public IActionResult Delete(int id)
{
    var item = _repo.GetById(id);
    if (item == null) return NotFound();
    return View(item);
}

// POST /Entity/Delete/5
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public IActionResult DeleteConfirmed(int id)
{
    _repo.Delete(id);
    return RedirectToAction(nameof(Index));
}
```

## Dropdown Helper Pattern

Centralize SelectList population in a private method to avoid repetition between GET and failed POST:

```csharp
private void PopulateDropdowns(Entity? model = null)
{
    ViewBag.Salons = new SelectList(_salonRepo.GetAll(), "Id", "Name", model?.HairSalonId);
    ViewBag.Categories = new SelectList(Enum.GetValues<ServiceCategory>(), model?.Category);
}
```

Call in both the GET action and any POST action path that returns `View(model)`.

## Server-Controlled Fields

Set these in the POST action, not in the form (users must not be able to tamper with them):

| Field | Where to set |
|---|---|
| `CreatedAt` | POST Create action: `model.CreatedAt = DateTime.UtcNow;` |
| `Id` | Never set manually — EF IDENTITY assigns it |
| Navigation properties | Load separately via repository after save if needed |

## ModelState Removal for Navigation Properties

If EF navigation properties are included in the model and cause ModelState errors (because they're not submitted by the form), remove them before validation:

```csharp
ModelState.Remove(nameof(Service.HairSalon));  // navigation prop, not in form
if (!ModelState.IsValid) ...
```

## Anti-Forgery Token

Every POST action that mutates data **must** have:
- `[ValidateAntiForgeryToken]` on the action
- `@Html.AntiForgeryToken()` inside the `<form>` in the view (or use `asp-action` tag helper which adds it automatically)
