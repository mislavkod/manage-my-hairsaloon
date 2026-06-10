using manage_my_hairsaloon.Models;
using manage_my_hairsaloon.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace manage_my_hairsaloon.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly IReservationRepository _reservationRepo;
        private readonly IUserRepository _userRepo;
        private readonly IStaffRepository _staffRepo;

        public ReservationsController(IReservationRepository reservationRepo, IUserRepository userRepo, IStaffRepository staffRepo)
        {
            _reservationRepo = reservationRepo;
            _userRepo = userRepo;
            _staffRepo = staffRepo;
        }

        [Authorize]
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
                return View(_reservationRepo.GetAll());

            var email = User.FindFirstValue(ClaimTypes.Email);
            var businessUser = email != null ? _userRepo.GetByEmail(email) : null;
            if (businessUser == null)
                return View(new List<Reservation>());

            if (User.IsInRole("Staff"))
            {
                var staffRecord = _staffRepo.GetByUserId(businessUser.Id);
                if (staffRecord != null)
                    return View(_reservationRepo.GetBySalonId(staffRecord.HairSalonId));
                return View(new List<Reservation>());
            }

            return View(_reservationRepo.GetByCustomerId(businessUser.Id));
        }

        [Authorize]
        public IActionResult Details(int id)
        {
            var reservation = _reservationRepo.GetById(id);
            if (reservation == null) return NotFound();
            return View(reservation);
        }

        // Primjer 3: atributni routing s enum parametrom + route constraint
        // Dostupno na: /reservations/status/Confirmed
        [Authorize]
        [HttpGet("reservations/status/{status}")]
        public IActionResult ByStatus(ReservationStatus status)
        {
            var reservations = _reservationRepo.GetByStatus(status);
            ViewBag.ActiveStatus = status.ToString();
            return View("Index", reservations);
        }

        // Ruta po salonu i statusu
        // Dostupno na: /salons/1/reservations/status/Confirmed
        [Authorize]
        [HttpGet("salons/{salonId}/reservations/status/{status}")]
        public IActionResult BySalonAndStatus(int salonId, ReservationStatus status)
        {
            var reservations = _reservationRepo.GetBySalonIdAndStatus(salonId, status);
            return View("Index", reservations);
        }

        // GET: /reservations/filter  — AJAX endpoint, returns partial HTML rows
        [Authorize]
        [HttpGet("reservations/filter")]
        public IActionResult Filter(string? status, string? customerName, string? serviceName,
                                    DateTime? dateFrom, DateTime? dateTo)
        {
            var reservations = _reservationRepo.Filter(status, customerName, serviceName, dateFrom, dateTo);
            return PartialView("_ReservationsTable", reservations);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _reservationRepo.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
