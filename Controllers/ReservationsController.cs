using manage_my_hairsaloon.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace manage_my_hairsaloon.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly MockReservationRepository _reservationRepo;

        public ReservationsController(MockReservationRepository reservationRepo)
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
    }
}
