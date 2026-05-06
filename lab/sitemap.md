# Sitemap — manage-my-hairsaloon

All available URLs, the controller + action that handles each, the repository methods called, and the view rendered.

---

## Convention Routes

Handled by the default route pattern `{Controller}/{Action}/{id?}` defined in `Program.cs`.

### Home

| URL | Method | Controller | Action | Repository calls | View | Model type |
|---|---|---|---|---|---|---|
| `/` or `/Home/Index` | GET | `HomeController` | `Index` | — | `Home/Index.cshtml` | none |
| `/Home/Privacy` | GET | `HomeController` | `Privacy` | — | `Home/Privacy.cshtml` | none |
| `/Home/Error` | GET | `HomeController` | `Error` | — | `Shared/Error.cshtml` | `ErrorViewModel` |

---

### HairSalons

| URL | Method | Controller | Action | Repository calls | View | Model type |
|---|---|---|---|---|---|---|
| `/HairSalons` or `/HairSalons/Index` | GET | `HairSalonsController` | `Index` | `GetAll()` | `HairSalons/Index.cshtml` | `List<HairSalon>` |
| `/HairSalons/Details/{id}` | GET | `HairSalonsController` | `Details` | `GetById(id)` | `HairSalons/Details.cshtml` | `HairSalon` |

---

### Staff

| URL | Method | Controller | Action | Repository calls | View | Model type |
|---|---|---|---|---|---|---|
| `/Staff` or `/Staff/Index` | GET | `StaffController` | `Index` | `GetAll()` | `Staff/Index.cshtml` | `List<Staff>` |
| `/Staff/Details/{id}` | GET | `StaffController` | `Details` | `GetById(id)` | `Staff/Details.cshtml` | `Staff` |

---

### Services

| URL | Method | Controller | Action | Repository calls | View | Model type |
|---|---|---|---|---|---|---|
| `/Services` or `/Services/Index` | GET | `ServicesController` | `Index` | `GetAll()` | `Services/Index.cshtml` | `List<Service>` |
| `/Services/Details/{id}` | GET | `ServicesController` | `Details` | `GetById(id)` | `Services/Details.cshtml` | `Service` |

---

### Reservations

| URL | Method | Controller | Action | Repository calls | View | Model type |
|---|---|---|---|---|---|---|
| `/Reservations` or `/Reservations/Index` | GET | `ReservationsController` | `Index` | `GetAll()` | `Reservations/Index.cshtml` | `List<Reservation>` |
| `/Reservations/Details/{id}` | GET | `ReservationsController` | `Details` | `GetById(id)` | `Reservations/Details.cshtml` | `Reservation` |

---

### Reviews

| URL | Method | Controller | Action | Repository calls | View | Model type |
|---|---|---|---|---|---|---|
| `/Reviews` or `/Reviews/Index` | GET | `ReviewsController` | `Index` | `GetAll()` | `Reviews/Index.cshtml` | `List<Review>` |
| `/Reviews/Details/{id}` | GET | `ReviewsController` | `Details` | `GetById(id)` | `Reviews/Details.cshtml` | `Review` |

---

### Users

| URL | Method | Controller | Action | Repository calls | View | Model type |
|---|---|---|---|---|---|---|
| `/Users` or `/Users/Index` | GET | `UsersController` | `Index` | `GetAll()` | `Users/Index.cshtml` | `List<User>` |
| `/Users/Details/{id}` | GET | `UsersController` | `Details` | `GetById(id)` | `Users/Details.cshtml` | `User` |

---

## Custom Attribute Routes

Handled by `[HttpGet("...")]` attributes on individual actions. These override the default convention for that action only.

| URL | Method | Controller | Action | Repository calls | View | Model type |
|---|---|---|---|---|---|---|
| `/salons/search/{name}` | GET | `HairSalonsController` | `Search` | `GetByName(name)` | `HairSalons/Details.cshtml` | `HairSalon` |
| `/Staff/Available` | GET | `StaffController` | `Available` | `GetAvailable()` | `Staff/Index.cshtml` | `List<Staff>` |
| `/services/category/{category}` | GET | `ServicesController` | `ByCategory` | `GetByCategory(category)` | `Services/Index.cshtml` | `List<Service>` |
| `/reservations/status/{status}` | GET | `ReservationsController` | `ByStatus` | `GetByStatus(status)` | `Reservations/Index.cshtml` | `List<Reservation>` |
| `/salons/{salonId}/staff/available` | GET | `StaffController` | `AvailableBySalon` | `GetAvailableBySalonId(salonId)` | `Staff/Index.cshtml` | `List<Staff>` |
| `/salons/{salonId}/reservations/status/{status}` | GET | `ReservationsController` | `BySalonAndStatus` | `GetBySalonIdAndStatus(salonId, status)` | `Reservations/Index.cshtml` | `List<Reservation>` |
| `/salons/{salonId}/NewReservations` | GET | `HairSalonsController` | `NewReservation` | `GetById`, `GetBySalonId`, `GetAvailableBySalonId`, `GetCustomers`, `GetBySalonIdAndDate` | `HairSalons/NewReservations.cshtml` | `NewReservationViewModel` |
| `/salons/{salonId}/NewReservations` | POST | `HairSalonsController` | `NewReservation` | same as GET + `Add(reservation)` | redirect → `/HairSalons/Details/{id}` | — |

---

## URL Parameters

| Parameter | Type | Valid values | Example |
|---|---|---|---|
| `{id}` | `int` | any existing entity ID | `/Staff/Details/3` |
| `{salonId}` | `int` | existing `HairSalon.Id` | `/salons/1/staff/available` |
| `{name}` | `string` | exact salon name (URL-encoded) | `/salons/search/Premium%20Cuts%20Downtown` |
| `{category}` | `ServiceCategory` | `HairCut`, `Coloring`, `Styling`, `Treatment`, `Beard`, `Nails`, `FacialTreatment` | `/services/category/Beard` |
| `{status}` | `ReservationStatus` | `Pending`, `Confirmed`, `Completed`, `Cancelled`, `NoShow` | `/reservations/status/Confirmed` |
| `?date` | `DateTime?` | ISO date string `yyyy-MM-dd` | `/salons/1/NewReservations?date=2026-05-10` |

---

## View Files — Full Inventory

| View file | @model |
|---|---|
| `Views/Home/Index.cshtml` | none |
| `Views/Home/Privacy.cshtml` | none |
| `Views/Shared/_Layout.cshtml` | none (layout) |
| `Views/Shared/Error.cshtml` | `ErrorViewModel` |
| `Views/HairSalons/Index.cshtml` | `List<HairSalon>` |
| `Views/HairSalons/Details.cshtml` | `HairSalon` |
| `Views/HairSalons/NewReservations.cshtml` | `NewReservationViewModel` |
| `Views/Staff/Index.cshtml` | `List<Staff>` |
| `Views/Staff/Details.cshtml` | `Staff` |
| `Views/Services/Index.cshtml` | `List<Service>` |
| `Views/Services/Details.cshtml` | `Service` |
| `Views/Reservations/Index.cshtml` | `List<Reservation>` |
| `Views/Reservations/Details.cshtml` | `Reservation` |
| `Views/Reviews/Index.cshtml` | `List<Review>` |
| `Views/Reviews/Details.cshtml` | `Review` |
| `Views/Users/Index.cshtml` | `List<User>` |
| `Views/Users/Details.cshtml` | `User` |

---

## Navigation Links (navbar)

Defined in `Views/Shared/_Layout.cshtml`. Always visible in the top nav:

| Label | Target URL |
|---|---|
| ✂ The Gentleman's Parlour | `/` |
| Home | `/Home/Index` |
| Hair Salons | `/HairSalons/Index` |
| Staff | `/Staff/Index` |
| Services | `/Services/Index` |
| Reservations | `/Reservations/Index` |
| Users | `/Users/Index` |
| Reviews | `/Reviews/Index` |

---

## In-Page Filter Links (not in navbar)

These are rendered inline on each index or detail page and link to custom attribute routes.

### Services/Index
| Button label | Target URL |
|---|---|
| All | `/Services/Index` |
| HairCut | `/services/category/HairCut` |
| Coloring | `/services/category/Coloring` |
| Styling | `/services/category/Styling` |
| Treatment | `/services/category/Treatment` |
| Beard | `/services/category/Beard` |
| Nails | `/services/category/Nails` |
| FacialTreatment | `/services/category/FacialTreatment` |

### Staff/Index
| Button label | Target URL |
|---|---|
| All Staff | `/Staff/Index` |
| Available Only | `/Staff/Available` |

### Reservations/Index
| Button label | Target URL |
|---|---|
| All | `/Reservations/Index` |
| Pending | `/reservations/status/Pending` |
| Confirmed | `/reservations/status/Confirmed` |
| Completed | `/reservations/status/Completed` |
| Cancelled | `/reservations/status/Cancelled` |
| NoShow | `/reservations/status/NoShow` |

### HairSalons/Index
| Control | Behaviour |
|---|---|
| Name filter input | Client-side JS — filters table rows by salon name without a page load |

### HairSalons/Details (salon-scoped)
| Button label | Target URL |
|---|---|
| Available Staff Only | `/salons/{id}/staff/available` |
| Pending | `/salons/{id}/reservations/status/Pending` |
| Confirmed | `/salons/{id}/reservations/status/Confirmed` |
| Completed | `/salons/{id}/reservations/status/Completed` |
| Cancelled | `/salons/{id}/reservations/status/Cancelled` |
| NoShow | `/salons/{id}/reservations/status/NoShow` |
| Make a Reservation | `/salons/{id}/NewReservations` |
