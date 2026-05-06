using manage_my_hairsaloon.Models;
using manage_my_hairsaloon.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace manage_my_hairsaloon.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly IReservationRepository _reservationRepo;

        public ReservationsController(IReservationRepository reservationRepo)
        {
            _reservationRepo = reservationRepo;
        }

        public IActionResult Index()
        {
            var reservations = _reservationRepo.GetAll();
            return View(reservations);
        }

        public IActionResult Details(int id)
        {
            var reservation = _reservationRepo.GetById(id);
            if (reservation == null) return NotFound();
            return View(reservation);
        }

        // Primjer 3: atributni routing s enum parametrom + route constraint
        // Dostupno na: /reservations/status/Confirmed
        [HttpGet("reservations/status/{status}")]
        public IActionResult ByStatus(ReservationStatus status)
        {
            var reservations = _reservationRepo.GetByStatus(status);
            return View("Index", reservations);
        }

        // Ruta po salonu i statusu
        // Dostupno na: /salons/1/reservations/status/Confirmed
        [HttpGet("salons/{salonId}/reservations/status/{status}")]
        public IActionResult BySalonAndStatus(int salonId, ReservationStatus status)
        {
            var reservations = _reservationRepo.GetBySalonIdAndStatus(salonId, status);
            return View("Index", reservations);
        }
    }
}
