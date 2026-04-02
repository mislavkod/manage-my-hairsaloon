# Hair Salon Management System - Data Models

This document explains all the models (entities) in the Hair Salon Management System and their relationships.

## Overview

The system consists of 7 core models organized into:
- **User Management**: User, UserRole
- **Business**: HairSalon, Staff, Service, ServiceCategory
- **Bookings**: Reservation, ReservationStatus
- **Reviews**: Review

---

## Models

### 1. User
**Purpose**: Represents all users in the system (customers, staff, admins)

**Properties**:
```csharp
public int Id { get; set; }
public string Email { get; set; }
public string PasswordHash { get; set; }
public string FirstName { get; set; }
public string LastName { get; set; }
public string PhoneNumber { get; set; }
public UserRole Role { get; set; }  // Enum: Customer, Staff, Admin
public DateTime CreatedAt { get; set; }
```

**Navigation Properties** (Related Data):
- `Staff` - If user is staff, links to Staff entity (1-to-1)
- `CustomerReservations` - Reservations made by this customer (1-to-many)
- `Reviews` - Reviews written by this customer (1-to-many)

**Example**:
```csharp
var user = new User 
{ 
    FirstName = "John", 
    Email = "john@email.com", 
    Role = UserRole.Customer 
};
```

---

### 2. UserRole (Enum)
**Purpose**: Defines the role/type of user

**Values**:
```csharp
Customer  // Can book reservations
Staff     // Works at salon
Admin     // Manages salon
```

**Usage**:
```csharp
if (user.Role == UserRole.Staff) { /* staff only logic */ }
```

---

### 3. HairSalon
**Purpose**: Represents a physical hair salon location

**Properties**:
```csharp
public int Id { get; set; }
public string Name { get; set; }
public string Address { get; set; }
public string PhoneNumber { get; set; }
public string Email { get; set; }
public DateTime CreatedAt { get; set; }
```

**Navigation Properties**:
- `Staff` - All staff members working at this salon (1-to-many)
- `Services` - All services offered by this salon (1-to-many)

**Relationships**:
- ONE salon has MANY staff members
- ONE salon has MANY services

**Example**:
```csharp
var salon = new HairSalon 
{ 
    Name = "Premium Cuts Downtown", 
    Address = "123 Main Street" 
};
salon.Staff.Add(staffMember);
salon.Services.Add(service);
```

---

### 4. Staff
**Purpose**: Represents a staff member at a salon

**Properties**:
```csharp
public int Id { get; set; }
public int UserId { get; set; }              // Foreign Key
public int HairSalonId { get; set; }         // Foreign Key
public string Specialization { get; set; }   // e.g., "Hair Coloring"
public decimal HourlyRate { get; set; }
public bool IsAvailable { get; set; }
```

**Navigation Properties**:
- `User` - Links to User entity (1-to-1)
- `HairSalon` - Links to HairSalon entity (many-to-1)
- `Reservations` - All reservations for this staff member (1-to-many)

**Relationships**:
- ONE user can be ONE staff member (one-to-one)
- MANY staff members work at ONE salon (many-to-one)
- ONE staff member has MANY reservations (one-to-many)

**Example**:
```csharp
var staff = new Staff 
{ 
    UserId = 5,
    HairSalonId = 1,
    Specialization = "Hair Coloring",
    HourlyRate = 50m
};
```

---

### 5. Service
**Purpose**: Represents a service offered by a salon

**Properties**:
```csharp
public int Id { get; set; }
public int HairSalonId { get; set; }         // Foreign Key
public string Name { get; set; }
public string Description { get; set; }
public decimal Price { get; set; }
public int DurationMinutes { get; set; }
public ServiceCategory Category { get; set; }  // Enum
```

**Navigation Properties**:
- `HairSalon` - Salon offering this service (many-to-1)
- `Reservations` - All reservations for this service (1-to-many)

**Relationships**:
- MANY services belong to ONE salon (many-to-one)
- ONE service has MANY reservations (one-to-many)

**Example**:
```csharp
var service = new Service 
{ 
    HairSalonId = 1,
    Name = "Hair Coloring",
    Price = 120m,
    DurationMinutes = 120,
    Category = ServiceCategory.Coloring
};
```

---

### 6. ServiceCategory (Enum)
**Purpose**: Categorizes types of services

**Values**:
```csharp
HairCut
Coloring
Styling
Treatment
Beard
Nails
FacialTreatment
```

**Usage**:
```csharp
if (service.Category == ServiceCategory.HairCut) { /* ... */ }
```

---

### 7. Reservation
**Purpose**: Represents a booking made by a customer

**Properties**:
```csharp
public int Id { get; set; }
public int CustomerId { get; set; }          // Foreign Key (User)
public int StaffId { get; set; }             // Foreign Key
public int ServiceId { get; set; }           // Foreign Key
public DateTime ReservationDateTime { get; set; }
public ReservationStatus Status { get; set; }  // Enum
public string Notes { get; set; }
public DateTime CreatedAt { get; set; }
```

**Navigation Properties**:
- `Customer` - The customer making the reservation (many-to-1 User)
- `Staff` - Staff member assigned to reservation (many-to-1)
- `Service` - Service being booked (many-to-1)
- `Reviews` - Reviews for this reservation (1-to-many)

**Relationships**:
- MANY reservations belong to ONE customer (many-to-one with User)
- MANY reservations assigned to ONE staff (many-to-one)
- MANY reservations for ONE service (many-to-one)
- ONE reservation has MANY reviews (one-to-many)

**Example**:
```csharp
var reservation = new Reservation 
{ 
    CustomerId = 1,
    StaffId = 2,
    ServiceId = 3,
    ReservationDateTime = new DateTime(2026, 4, 5, 10, 0, 0),
    Status = ReservationStatus.Confirmed,
    Notes = "First time customer"
};
```

---

### 8. ReservationStatus (Enum)
**Purpose**: Tracks the status of a reservation

**Values**:
```csharp
Pending      // Waiting for confirmation
Confirmed    // Confirmed appointment
Completed    // Service completed
Cancelled    // Customer cancelled
NoShow       // Customer didn't show up
```

**Usage**:
```csharp
var confirmed = reservations
    .Where(r => r.Status == ReservationStatus.Confirmed)
    .ToList();
```

---

### 9. Review
**Purpose**: Customer reviews for completed services

**Properties**:
```csharp
public int Id { get; set; }
public int ReservationId { get; set; }       // Foreign Key
public int CustomerId { get; set; }          // Foreign Key (User)
public int Rating { get; set; }              // 1-5 stars
public string Comment { get; set; }
public DateTime CreatedAt { get; set; }
```

**Navigation Properties**:
- `Reservation` - The reservation being reviewed (many-to-1)
- `Customer` - The customer who wrote the review (many-to-1 User)

**Relationships**:
- MANY reviews for ONE reservation (many-to-one)
- MANY reviews written by ONE customer (many-to-one with User)

**Example**:
```csharp
var review = new Review 
{ 
    ReservationId = 5,
    CustomerId = 1,
    Rating = 5,
    Comment = "Excellent service!"
};
```

---

## Entity Relationship Diagram

```
User (1) ──────→ (1) Staff ──────→ (1) HairSalon
  ↑                     ↓
  │              (many) Reservations
  │                     ↓
  └─────────────────────┴─────────→ (1) Service

Reservation (1) ──────→ (many) Reviews
                              ↓
                         Links to User & Reservation
```

**Detailed Relationships**:

```
HairSalon (1) ────────→ (many) Staff
                              ↓
                        Works at salon
                        
            ────────→ (many) Services
                     Offered by salon

Staff (1) ──────→ (many) Reservations
                    ↓
User (Customer) ─←─ Reservation ─→ Service
(1)        (many)        ↓
                    (1) Review (many)
```

---

## Key Relationships Summary

| From | To | Type | Meaning |
|------|----|----|---------|
| User | Staff | 1-to-1 | A user can be one staff member |
| User | Reservation | 1-to-many | A customer can have many reservations |
| User | Review | 1-to-many | A customer can write many reviews |
| HairSalon | Staff | 1-to-many | A salon has many staff members |
| HairSalon | Service | 1-to-many | A salon offers many services |
| Staff | Reservation | 1-to-many | A staff member has many reservations |
| Service | Reservation | 1-to-many | A service can have many reservations |
| Reservation | Review | 1-to-many | A reservation can have many reviews |

---

## Sample Data Flow

### Creating a Reservation

```csharp
// 1. Customer (User with Role.Customer)
var customer = new User 
{ 
    FirstName = "John", 
    Role = UserRole.Customer 
};

// 2. Salon
var salon = new HairSalon { Name = "Premium Cuts" };

// 3. Staff at Salon
var staff = new Staff 
{ 
    User = customer,  // Actually different user
    HairSalon = salon 
};

// 4. Service at Salon
var service = new Service 
{ 
    HairSalon = salon,
    Name = "Hair Cut",
    Price = 60m
};

// 5. Reservation
var reservation = new Reservation 
{ 
    Customer = customer,
    Staff = staff,
    Service = service,
    ReservationDateTime = new DateTime(2026, 4, 5, 10, 0, 0),
    Status = ReservationStatus.Confirmed
};

// 6. Review (after service completed)
var review = new Review 
{ 
    Reservation = reservation,
    Customer = customer,
    Rating = 5,
    Comment = "Great service!"
};
```

---

## LINQ Query Examples Using Models

```csharp
// Get all reservations for a specific customer
var customerReservations = reservations
    .Where(r => r.Customer.Id == customerId)
    .ToList();

// Get staff with highest hourly rate
var topStaff = staff
    .OrderByDescending(s => s.HourlyRate)
    .First();

// Get average rating for a service
var averageRating = reviews
    .Where(r => r.Reservation.Service.Id == serviceId)
    .Average(r => r.Rating);

// Get all pending reservations for a salon
var pendingReservations = reservations
    .Where(r => r.Status == ReservationStatus.Pending 
        && r.Staff.HairSalon.Id == salonId)
    .OrderBy(r => r.ReservationDateTime)
    .ToList();
```

---

## Notes

- **Foreign Keys**: Fields ending in `Id` (UserId, HairSalonId, etc.) link to other entities
- **Navigation Properties**: Non-primitive properties that reference other entities (User, Staff, etc.)
- **Enums**: Used for fixed categories (Roles, Statuses, Categories)
- **DateTime**: All timestamps use `DateTime.Now` for creation tracking
- **Decimal**: Used for prices (better precision than float)

