using manage_my_hairsaloon.Models;
using manage_my_hairsaloon.Repositories;
using manage_my_hairsaloon.ViewModels;
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
            ViewBag.MaxPrice = services.Any() ? services.Max(s => s.Price) : 100m;
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
            ViewBag.ActiveCategory = category.ToString();
            ViewBag.MaxPrice = _serviceRepo.GetMaxPrice();
            return View("Index", services);
        }

        // GET: /services/filter  — AJAX endpoint, returns partial HTML rows
        [HttpGet("services/filter")]
        public IActionResult Filter(string? category, string? serviceName, string? salonName, decimal? maxPrice)
        {
            var services = _serviceRepo.Filter(category, serviceName, salonName, maxPrice);
            return PartialView("_ServicesTable", services);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var service = _serviceRepo.GetById(id);
            if (service == null) return NotFound();

            var vm = new EditServiceViewModel
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                DurationMinutes = service.DurationMinutes,
                Category = service.Category,
                OriginalName = service.Name,
                OriginalDescription = service.Description,
                OriginalPrice = service.Price,
                OriginalDurationMinutes = service.DurationMinutes,
                OriginalCategory = service.Category,
                SalonName = service.HairSalon?.Name
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, EditServiceViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var current = _serviceRepo.GetById(id);
            if (current == null) return NotFound();

            // Optimistic concurrency check
            bool conflict =
                current.Name != vm.OriginalName ||
                current.Description != vm.OriginalDescription ||
                current.Price != vm.OriginalPrice ||
                current.DurationMinutes != vm.OriginalDurationMinutes ||
                current.Category != vm.OriginalCategory;

            if (conflict)
            {
                ModelState.AddModelError(string.Empty,
                    "This record was modified by someone else. The current values are shown below. Please review and resubmit.");
                vm.OriginalName = current.Name;
                vm.OriginalDescription = current.Description;
                vm.OriginalPrice = current.Price;
                vm.OriginalDurationMinutes = current.DurationMinutes;
                vm.OriginalCategory = current.Category;
                vm.Name = current.Name;
                vm.Description = current.Description;
                vm.Price = current.Price;
                vm.DurationMinutes = current.DurationMinutes;
                vm.Category = current.Category;
                vm.SalonName = current.HairSalon?.Name;
                return View(vm);
            }

            current.Name = vm.Name;
            current.Description = vm.Description;
            current.Price = vm.Price;
            current.DurationMinutes = vm.DurationMinutes;
            current.Category = vm.Category;
            _serviceRepo.Update(current);

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _serviceRepo.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
