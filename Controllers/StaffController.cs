using manage_my_hairsaloon.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace manage_my_hairsaloon.Controllers
{
    public class StaffController : Controller
    {
        private readonly IStaffRepository _staffRepo;

        public StaffController(IStaffRepository staffRepo)
        {
            _staffRepo = staffRepo;
        }

        public IActionResult Index()
        {
            var staff = _staffRepo.GetAll();
            return View(staff);
        }

        public IActionResult Details(int id)
        {
            var staffMember = _staffRepo.GetById(id);
            if (staffMember == null) return NotFound();
            return View(staffMember);
        }

        // Primjer 2: fiksna slug ruta bez parametara
        // Dostupno na: /Staff/Available
        [HttpGet("Staff/Available")]
        public IActionResult Available()
        {
            var available = _staffRepo.GetAvailable();
            return View("Index", available);
        }

        // Dostupno osoblje po salonu
        // Dostupno na: /salons/1/staff/available
        [HttpGet("salons/{salonId}/staff/available")]
        public IActionResult AvailableBySalon(int salonId)
        {
            var available = _staffRepo.GetAvailableBySalonId(salonId);
            return View("Index", available);
        }
    }
}
