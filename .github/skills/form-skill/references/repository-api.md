# Repository API Reference

All repositories are registered as `Scoped` in `Program.cs`. Inject via constructor using the interface type, never the concrete class or `AppDbContext` directly.

## IHairSalonRepository

```csharp
List<HairSalon> GetAll();
HairSalon? GetById(int id);
HairSalon? GetByName(string name);
// Write methods: Add / Update / Delete — NOT YET ADDED, add them when needed
```

## IStaffRepository

```csharp
List<Staff> GetAll();
Staff? GetById(int id);
List<Staff> GetBySalonId(int salonId);
Staff? GetByUserId(int userId);
List<Staff> GetAvailable();
List<Staff> GetAvailableBySalonId(int salonId);
// Write methods: NOT YET ADDED
```

## IServiceRepository

```csharp
List<Service> GetAll();
Service? GetById(int id);
List<Service> GetBySalonId(int salonId);
List<Service> GetByCategory(ServiceCategory category);
List<Service> GetByMaxPrice(decimal maxPrice);
// Write methods: NOT YET ADDED
```

## IReservationRepository

```csharp
List<Reservation> GetAll();
Reservation? GetById(int id);
List<Reservation> GetByCustomerId(int customerId);
List<Reservation> GetByStaffId(int staffId);
List<Reservation> GetByStatus(ReservationStatus status);
List<Reservation> GetByServiceId(int serviceId);
List<Reservation> GetBySalonIdAndStatus(int salonId, ReservationStatus status);
// Write methods: NOT YET ADDED
```

## IReviewRepository

```csharp
List<Review> GetAll();
Review? GetById(int id);
// Write methods: NOT YET ADDED
```

## IUserRepository

```csharp
List<User> GetAll();
User? GetById(int id);
// Write methods: NOT YET ADDED
```

---

## Adding a Write Method

When a form needs to persist data, follow these steps:

1. **Add the method signature** to the interface (`Repositories/I{Entity}Repository.cs`)
2. **Implement it** in `Repositories/{Entity}Repository.cs`

Minimal EF implementation patterns:

```csharp
// Add
public void Add(T entity)
{
    _context.Set<T>().Add(entity);
    _context.SaveChanges();
}

// Update
public void Update(T entity)
{
    _context.Set<T>().Update(entity);
    _context.SaveChanges();
}

// Delete by id
public void Delete(int id)
{
    var entity = _context.Set<T>().Find(id);
    if (entity != null)
    {
        _context.Set<T>().Remove(entity);
        _context.SaveChanges();
    }
}
```

> Always call `SaveChanges()` inside the repository method. Controllers must not call it directly.

## Enum Types

```csharp
// ServiceCategory
HairCut, Coloring, Styling, Treatment, Beard, Nails, FacialTreatment

// ReservationStatus
Pending, Confirmed, Completed, Cancelled, NoShow

// UserRole
Customer, Staff, Admin
```
