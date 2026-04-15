using manage_my_hairsaloon.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace manage_my_hairsaloon.Controllers
{
    public class StaffController : Controller
    {
        private readonly MockStaffRepository _staffRepo;

        public StaffController(MockStaffRepository staffRepo)
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
    }
}
