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
    }
}
