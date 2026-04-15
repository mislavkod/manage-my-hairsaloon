using manage_my_hairsaloon.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace manage_my_hairsaloon.Controllers
{
    public class HairSalonsController : Controller
    {
        private readonly MockHairSalonRepository _salonRepo;

        public HairSalonsController(MockHairSalonRepository salonRepo)
        {
            _salonRepo = salonRepo;
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
    }
}
