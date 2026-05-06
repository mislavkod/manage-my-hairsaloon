using manage_my_hairsaloon.Models;
using manage_my_hairsaloon.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace manage_my_hairsaloon.Controllers
{
    public class ServicesController : Controller
    {
        private readonly IServiceRepository _serviceRepo;

        public ServicesController(IServiceRepository serviceRepo)
        {
            _serviceRepo = serviceRepo;
        }

        public IActionResult Index()
        {
            var services = _serviceRepo.GetAll();
            return View(services);
        }

        public IActionResult Details(int id)
        {
            var service = _serviceRepo.GetById(id);
            if (service == null) return NotFound();
            return View(service);
        }

        // Primjer 1: atributni routing s enum parametrom u URL-u
        // Dostupno na: /services/category/HairCut
        [HttpGet("services/category/{category}")]
        public IActionResult ByCategory(ServiceCategory category)
        {
            var services = _serviceRepo.GetByCategory(category);
            return View("Index", services);
        }
    }
}
