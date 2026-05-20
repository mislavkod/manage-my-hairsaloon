using manage_my_hairsaloon.Models;
using manage_my_hairsaloon.Repositories;
using manage_my_hairsaloon.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace manage_my_hairsaloon.Controllers
{
    public class HairSalonsController : Controller
    {
        private readonly IHairSalonRepository _salonRepo;
        private readonly IServiceRepository _serviceRepo;
        private readonly IStaffRepository _staffRepo;
        private readonly IReservationRepository _reservationRepo;
        private readonly IUserRepository _userRepo;

        // Working hours: 09:00 – 18:00 (inclusive start hour)
        private static readonly int[] WorkHours = Enumerable.Range(9, 10).ToArray();

        public HairSalonsController(
            IHairSalonRepository salonRepo,
            IServiceRepository serviceRepo,
            IStaffRepository staffRepo,
            IReservationRepository reservationRepo,
            IUserRepository userRepo)
        {
            _salonRepo = salonRepo;
            _serviceRepo = serviceRepo;
            _staffRepo = staffRepo;
            _reservationRepo = reservationRepo;
            _userRepo = userRepo;
        }

        public IActionResult Index()
        {
            var salons = _salonRepo.GetAll();
            return View(salons);
        }

        public IActionResult Details(int id)
        {
            var salon = _salonRepo.GetById(id);
            if (salon == null) return NotFound();
            return View(salon);
        }

        // GET: /HairSalons/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new HairSalon());
        }

        // POST: /HairSalons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(HairSalon model)
        {
            ModelState.Remove(nameof(HairSalon.Staff));
            ModelState.Remove(nameof(HairSalon.Services));

            if (!ModelState.IsValid)
                return View(model);

            model.CreatedAt = DateTime.UtcNow;
            _salonRepo.Add(model);
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        // Primjer 4: drugačiji prefix u ruti od naziva kontrolera
        // Kontroler se zove HairSalons, ali ruta koristi kraći alias "salons"
        // Dostupno na: /salons/search/Premium
        [HttpGet("salons/search/{name}")]
        public IActionResult Search(string name)
        {
            var salon = _salonRepo.GetByName(name);
            if (salon == null) return NotFound();
            return View("Details", salon);
        }

        // GET: /salons/{salonId}/NewReservations?date=2026-05-10
        [HttpGet("salons/{salonId}/NewReservations")]
        public IActionResult NewReservation(int salonId, DateTime? date)
        {
            var salon = _salonRepo.GetById(salonId);
            if (salon == null) return NotFound();

            var vm = BuildViewModel(salonId, salon, date);
            return View("NewReservations", vm);
        }

        // POST: /salons/{salonId}/NewReservations
        [HttpPost("salons/{salonId}/NewReservations")]
        [ValidateAntiForgeryToken]
        public IActionResult NewReservation(int salonId, NewReservationViewModel vm)
        {
            var salon = _salonRepo.GetById(salonId);
            if (salon == null) return NotFound();

            // Repopulate display data before any return View()
            var rebuildVm = BuildViewModel(salonId, salon, vm.SelectedDate);
            rebuildVm.ServiceId = vm.ServiceId;
            rebuildVm.StaffId = vm.StaffId;
            rebuildVm.CustomerId = vm.CustomerId;
            rebuildVm.SelectedDate = vm.SelectedDate;
            rebuildVm.SelectedHour = vm.SelectedHour;
            rebuildVm.Notes = vm.Notes;

            // Remove navigation-only properties from validation
            ModelState.Remove(nameof(NewReservationViewModel.Salon));
            ModelState.Remove(nameof(NewReservationViewModel.Services));
            ModelState.Remove(nameof(NewReservationViewModel.AvailableStaff));
            ModelState.Remove(nameof(NewReservationViewModel.Customers));
            ModelState.Remove(nameof(NewReservationViewModel.AvailableHours));

            if (!ModelState.IsValid)
                return View("NewReservations", rebuildVm);

            // Validate the reservation date is not in the past
            if (vm.SelectedDate!.Value.Date < DateTime.Today)
            {
                ModelState.AddModelError(nameof(vm.SelectedDate), "Reservation date cannot be in the past.");
                return View("NewReservations", rebuildVm);
            }

            // Validate selected hour is still available
            if (!rebuildVm.AvailableHours.Contains(vm.SelectedHour!.Value))
            {
                ModelState.AddModelError(nameof(vm.SelectedHour), "The selected time slot is no longer available.");
                return View("NewReservations", rebuildVm);
            }

            var reservation = new Reservation
            {
                CustomerId = vm.CustomerId,
                StaffId = vm.StaffId,
                ServiceId = vm.ServiceId,
                ReservationDateTime = vm.SelectedDate!.Value.Date.AddHours(vm.SelectedHour!.Value),
                Status = ReservationStatus.Pending,
                Notes = vm.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _reservationRepo.Add(reservation);
            return RedirectToAction(nameof(Details), new { id = salonId });
        }

        // GET: /salons/{salonId}/NewStaff/NewUser
        [HttpGet("salons/{salonId}/NewStaff/NewUser")]
        public IActionResult NewStaffUser(int salonId)
        {
            var salon = _salonRepo.GetById(salonId);
            if (salon == null) return NotFound();

            ViewBag.Salon = salon;
            return View(new User { Role = UserRole.Staff });
        }

        // POST: /salons/{salonId}/NewStaff/NewUser
        [HttpPost("salons/{salonId}/NewStaff/NewUser")]
        [ValidateAntiForgeryToken]
        public IActionResult NewStaffUser(int salonId, User model, string? password)
        {
            var salon = _salonRepo.GetById(salonId);
            if (salon == null) return NotFound();

            ModelState.Remove(nameof(Models.User.Staff));
            ModelState.Remove(nameof(Models.User.CustomerReservations));
            ModelState.Remove(nameof(Models.User.Reviews));
            ModelState.Remove(nameof(Models.User.PasswordHash));

            if (string.IsNullOrWhiteSpace(password))
                ModelState.AddModelError("password", "Password is required.");

            if (!string.IsNullOrWhiteSpace(model.Email) && _userRepo.GetByEmail(model.Email) != null)
                ModelState.AddModelError(nameof(Models.User.Email), "An account with this email address already exists.");

            if (!ModelState.IsValid)
            {
                ViewBag.Salon = salon;
                return View(model);
            }

            model.Role = UserRole.Staff;
            model.CreatedAt = DateTime.UtcNow;
            model.PasswordHash = string.IsNullOrWhiteSpace(password) ? "placeholder" : password;
            _userRepo.Add(model);

            return RedirectToAction(nameof(NewStaff), new { salonId, selectedUserId = model.Id });
        }

        // GET: /salons/{salonId}/NewStaff
        [HttpGet("salons/{salonId}/NewStaff")]
        public IActionResult NewStaff(int salonId, int? selectedUserId)
        {
            var salon = _salonRepo.GetById(salonId);
            if (salon == null) return NotFound();

            ViewBag.Salon = salon;
            ViewBag.Users = _userRepo.GetStaffUsers();
            return View(new Staff { HairSalonId = salonId, UserId = selectedUserId ?? 0 });
        }

        // POST: /salons/{salonId}/NewStaff
        [HttpPost("salons/{salonId}/NewStaff")]
        [ValidateAntiForgeryToken]
        public IActionResult NewStaff(int salonId, Staff model)
        {
            var salon = _salonRepo.GetById(salonId);
            if (salon == null) return NotFound();

            ModelState.Remove(nameof(Staff.User));
            ModelState.Remove(nameof(Staff.HairSalon));
            ModelState.Remove(nameof(Staff.Reservations));

            if (model.UserId > 0 && _staffRepo.GetByUserId(model.UserId) != null)
                ModelState.AddModelError(nameof(Staff.UserId), "This user is already assigned as a staff member.");

            if (!ModelState.IsValid)
            {
                ViewBag.Salon = salon;
                ViewBag.Users = _userRepo.GetStaffUsers();
                model.HairSalonId = salonId;
                return View(model);
            }

            model.HairSalonId = salonId;
            _staffRepo.Add(model);
            return RedirectToAction(nameof(Details), new { id = salonId });
        }

        // GET: /salons/{salonId}/NewService
        [HttpGet("salons/{salonId}/NewService")]
        public IActionResult NewService(int salonId)
        {
            var salon = _salonRepo.GetById(salonId);
            if (salon == null) return NotFound();

            ViewBag.Salon = salon;
            ViewBag.Categories = Enum.GetValues<ServiceCategory>();
            return View(new Service { HairSalonId = salonId });
        }

        // POST: /salons/{salonId}/NewService
        [HttpPost("salons/{salonId}/NewService")]
        [ValidateAntiForgeryToken]
        public IActionResult NewService(int salonId, Service model)
        {
            var salon = _salonRepo.GetById(salonId);
            if (salon == null) return NotFound();

            ModelState.Remove(nameof(Service.HairSalon));
            ModelState.Remove(nameof(Service.Reservations));

            if (!ModelState.IsValid)
            {
                ViewBag.Salon = salon;
                ViewBag.Categories = Enum.GetValues<ServiceCategory>();
                model.HairSalonId = salonId;
                return View(model);
            }

            model.HairSalonId = salonId;
            _serviceRepo.Add(model);
            return RedirectToAction(nameof(Details), new { id = salonId });
        }

        // -------------------------------------------------------
        private NewReservationViewModel BuildViewModel(int salonId, HairSalon salon, DateTime? date)
        {
            var vm = new NewReservationViewModel
            {
                Salon = salon,
                Services = _serviceRepo.GetBySalonId(salonId),
                AvailableStaff = _staffRepo.GetAvailableBySalonId(salonId),
                Customers = _userRepo.GetCustomers(),
                SelectedDate = date
            };

            if (date.HasValue)
            {
                var staffIds = vm.AvailableStaff.Select(s => s.Id).ToHashSet();
                var bookedByHour = _reservationRepo
                    .GetBySalonIdAndDate(salonId, date.Value)
                    .GroupBy(r => r.ReservationDateTime.Hour)
                    .ToDictionary(g => g.Key, g => g.Select(r => r.StaffId).Distinct().Count());

                vm.AvailableHours = WorkHours
                    .Where(h => !bookedByHour.ContainsKey(h) || bookedByHour[h] < staffIds.Count)
                    .ToList();
            }

            return vm;
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var salon = _salonRepo.GetById(id);
            if (salon == null) return NotFound();

            var vm = new EditHairSalonViewModel
            {
                Id = salon.Id,
                Name = salon.Name,
                Address = salon.Address,
                PhoneNumber = salon.PhoneNumber,
                Email = salon.Email,
                OriginalName = salon.Name,
                OriginalAddress = salon.Address,
                OriginalPhoneNumber = salon.PhoneNumber,
                OriginalEmail = salon.Email
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, EditHairSalonViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var current = _salonRepo.GetById(id);
            if (current == null) return NotFound();

            bool conflict =
                current.Name != vm.OriginalName ||
                current.Address != vm.OriginalAddress ||
                current.PhoneNumber != vm.OriginalPhoneNumber ||
                current.Email != vm.OriginalEmail;

            if (conflict)
            {
                ModelState.AddModelError(string.Empty,
                    "This record was modified by someone else. The current values are shown below. Please review and resubmit.");
                vm.OriginalName = current.Name;
                vm.OriginalAddress = current.Address;
                vm.OriginalPhoneNumber = current.PhoneNumber;
                vm.OriginalEmail = current.Email;
                vm.Name = current.Name;
                vm.Address = current.Address;
                vm.PhoneNumber = current.PhoneNumber;
                vm.Email = current.Email;
                return View(vm);
            }

            current.Name = vm.Name;
            current.Address = vm.Address;
            current.PhoneNumber = vm.PhoneNumber;
            current.Email = vm.Email;
            _salonRepo.Update(current);

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _salonRepo.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
