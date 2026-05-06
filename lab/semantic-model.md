# Semantic Model — manage-my-hairsaloon

Properties and relationships for all 6 entity classes and 3 enums. All classes are in `Models/`.

---

## Entities

### User
| Property | Type | Notes |
|---|---|---|
| `Id` | `int` | PK, IDENTITY |
| `Email` | `string?` | — |
| `PasswordHash` | `string?` | — |
| `FirstName` | `string?` | — |
| `LastName` | `string?` | — |
| `PhoneNumber` | `string?` | — |
| `Role` | `UserRole` | Enum |
| `CreatedAt` | `DateTime` | — |
| `Staff` | `Staff?` | nav — 1-to-1, only set when `Role == Staff` |
| `CustomerReservations` | `List<Reservation>` | nav — reservations booked by this user |
| `Reviews` | `List<Review>` | nav — reviews written by this user |

---

### HairSalon
| Property | Type | Notes |
|---|---|---|
| `Id` | `int` | PK, IDENTITY |
| `Name` | `string?` | — |
| `Address` | `string?` | — |
| `PhoneNumber` | `string?` | — |
| `Email` | `string?` | — |
| `CreatedAt` | `DateTime` | — |
| `Staff` | `List<Staff>` | nav — staff employed at this salon |
| `Services` | `List<Service>` | nav — services offered at this salon |

---

### Staff
| Property | Type | Notes |
|---|---|---|
| `Id` | `int` | PK, IDENTITY |
| `UserId` | `int` | FK → `User.Id` |
| `HairSalonId` | `int` | FK → `HairSalon.Id` |
| `Specialization` | `string?` | — |
| `HourlyRate` | `decimal` | `decimal(10,2)` |
| `IsAvailable` | `bool` | — |
| `User` | `User?` | nav |
| `HairSalon` | `HairSalon?` | nav |
| `Reservations` | `List<Reservation>` | nav — appointments assigned to this staff member |

---

### Service
| Property | Type | Notes |
|---|---|---|
| `Id` | `int` | PK, IDENTITY |
| `HairSalonId` | `int` | FK → `HairSalon.Id` |
| `Name` | `string?` | — |
| `Description` | `string?` | — |
| `Price` | `decimal` | `decimal(10,2)` |
| `DurationMinutes` | `int` | — |
| `Category` | `ServiceCategory` | Enum |
| `HairSalon` | `HairSalon?` | nav |
| `Reservations` | `List<Reservation>` | nav |

---

### Reservation
| Property | Type | Notes |
|---|---|---|
| `Id` | `int` | PK, IDENTITY |
| `CustomerId` | `int` | FK → `User.Id` |
| `StaffId` | `int` | FK → `Staff.Id` |
| `ServiceId` | `int` | FK → `Service.Id` |
| `ReservationDateTime` | `DateTime` | date + hour of appointment |
| `Status` | `ReservationStatus` | Enum |
| `Notes` | `string?` | — |
| `CreatedAt` | `DateTime` | — |
| `Customer` | `User?` | nav |
| `Staff` | `Staff?` | nav |
| `Service` | `Service?` | nav |
| `Reviews` | `List<Review>` | nav |

---

### Review
| Property | Type | Notes |
|---|---|---|
| `Id` | `int` | PK, IDENTITY |
| `ReservationId` | `int` | FK → `Reservation.Id` |
| `CustomerId` | `int` | FK → `User.Id` |
| `Rating` | `int` | 1–5 |
| `Comment` | `string?` | — |
| `CreatedAt` | `DateTime` | — |
| `Reservation` | `Reservation?` | nav |
| `Customer` | `User?` | nav |

---

## Enums

### UserRole
| Value | Meaning |
|---|---|
| `Customer` | Can book reservations |
| `Staff` | Works at a salon |
| `Admin` | Manages the system |

### ServiceCategory
| Value |
|---|
| `HairCut` |
| `Coloring` |
| `Styling` |
| `Treatment` |
| `Beard` |
| `Nails` |
| `FacialTreatment` |

### ReservationStatus
| Value | Meaning |
|---|---|
| `Pending` | Awaiting confirmation |
| `Confirmed` | Appointment confirmed |
| `Completed` | Service delivered |
| `Cancelled` | Customer cancelled |
| `NoShow` | Customer did not show up |

---

## Relationships

| From | FK on | To | Cardinality | Delete behaviour |
|---|---|---|---|---|
| `Staff` | `Staff.UserId` | `User` | many-to-1 | Restrict |
| `Staff` | `Staff.HairSalonId` | `HairSalon` | many-to-1 | Restrict |
| `Service` | `Service.HairSalonId` | `HairSalon` | many-to-1 | Restrict |
| `Reservation` | `Reservation.CustomerId` | `User` | many-to-1 | Restrict |
| `Reservation` | `Reservation.StaffId` | `Staff` | many-to-1 | Restrict |
| `Reservation` | `Reservation.ServiceId` | `Service` | many-to-1 | Restrict |
| `Review` | `Review.ReservationId` | `Reservation` | many-to-1 | Cascade |
| `Review` | `Review.CustomerId` | `User` | many-to-1 | Restrict |

> All `Reservation` FK deletes are `Restrict` to avoid multiple cascade path conflicts in SQL Server.  
> Deleting a `Reservation` cascades to its `Review` rows.

---

## Traversal Paths

There is no direct `HairSalonId` on `Reservation`. To reach a salon's reservations, traverse via `Staff`:

```
HairSalon → Staff[] → Reservations[]
```

In C#:
```csharp
var salonReservations = salon.Staff
    .SelectMany(s => s.Reservations)
    .ToList();
```

In EF (repository query):
```csharp
context.Reservations
    .Where(r => r.Staff!.HairSalonId == salonId)
```

To reach a salon's reviews:
```
HairSalon → Staff[] → Reservations[] → Reviews[]
```

```csharp
var salonReviews = salon.Staff
    .SelectMany(s => s.Reservations)
    .SelectMany(r => r.Reviews)
    .ToList();
```

---

## Diagram

```
User (1) ─────────────────── (1) Staff ──────────── (1) HairSalon
  │                                 │                      │
  │ (CustomerReservations)          │ (Reservations)        │ (Services)
  ▼                                 ▼                      ▼
Reservation ◄────────────────────────────────────── Service
  │
  │ (Reviews)
  ▼
Review
```
