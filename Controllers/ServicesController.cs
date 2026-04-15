using manage_my_hairsaloon.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace manage_my_hairsaloon.Controllers
{
    public class ServicesController : Controller
    {
        private readonly MockServiceRepository _serviceRepo;

        public ServicesController(MockServiceRepository serviceRepo)
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
    }
}
