using manage_my_hairsaloon.Models;
using manage_my_hairsaloon.Repositories;
using manage_my_hairsaloon.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace manage_my_hairsaloon.Controllers
{
    public class ServicesController : Controller
    {
        private readonly IServiceRepository _serviceRepo;
        private readonly IUserRepository _userRepo;
        private readonly IStaffRepository _staffRepo;

        public ServicesController(IServiceRepository serviceRepo, IUserRepository userRepo, IStaffRepository staffRepo)
        {
            _serviceRepo = serviceRepo;
            _userRepo = userRepo;
            _staffRepo = staffRepo;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User.IsInRole("Staff"))
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                var businessUser = email != null ? _userRepo.GetByEmail(email) : null;
                var staffRecord = businessUser != null ? _staffRepo.GetByUserId(businessUser.Id) : null;
                if (staffRecord != null)
                {
                    var salonServices = _serviceRepo.GetBySalonId(staffRecord.HairSalonId);
                    ViewBag.MaxPrice = salonServices.Any() ? salonServices.Max(s => s.Price) : 100m;
                    return View(salonServices);
                }
            }
            var services = _serviceRepo.GetAll();
            ViewBag.MaxPrice = services.Any() ? services.Max(s => s.Price) : 100m;
            return View(services);
        }

        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var service = _serviceRepo.GetById(id);
            if (service == null) return NotFound();
            return View(service);
        }

        // Primjer 1: atributni routing s enum parametrom u URL-u
        // Dostupno na: /services/category/HairCut
        [AllowAnonymous]
        [HttpGet("services/category/{category}")]
        public IActionResult ByCategory(ServiceCategory category)
        {
            var services = _serviceRepo.GetByCategory(category);
            ViewBag.ActiveCategory = category.ToString();
            ViewBag.MaxPrice = _serviceRepo.GetMaxPrice();
            return View("Index", services);
        }

        // GET: /services/filter  — AJAX endpoint, returns partial HTML rows
        [AllowAnonymous]
        [HttpGet("services/filter")]
        public IActionResult Filter(string? category, string? serviceName, string? salonName, decimal? maxPrice)
        {
            var services = _serviceRepo.Filter(category, serviceName, salonName, maxPrice);
            return PartialView("_ServicesTable", services);
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _serviceRepo.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
