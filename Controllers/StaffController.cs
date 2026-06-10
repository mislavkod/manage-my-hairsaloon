using manage_my_hairsaloon.Models;
using manage_my_hairsaloon.Repositories;
using manage_my_hairsaloon.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace manage_my_hairsaloon.Controllers
{
    public class StaffController : Controller
    {
        private readonly IStaffRepository _staffRepo;
        private readonly IUserRepository _userRepo;

        public StaffController(IStaffRepository staffRepo, IUserRepository userRepo)
        {
            _staffRepo = staffRepo;
            _userRepo = userRepo;
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
                    return View(_staffRepo.GetBySalonId(staffRecord.HairSalonId));
            }
            return View(_staffRepo.GetAll());
        }

        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var staffMember = _staffRepo.GetById(id);
            if (staffMember == null) return NotFound();
            return View(staffMember);
        }

        // Primjer 2: fiksna slug ruta bez parametara
        // Dostupno na: /Staff/Available
        [AllowAnonymous]
        [HttpGet("Staff/Available")]
        public IActionResult Available()
        {
            var available = _staffRepo.GetAvailable();
            ViewBag.ActiveAvailability = "true";
            return View("Index", available);
        }

        // GET: /salons/{salonId}/staff/available
        [AllowAnonymous]
        [HttpGet("salons/{salonId}/staff/available")]
        public IActionResult AvailableBySalon(int salonId)
        {
            var available = _staffRepo.GetAvailableBySalonId(salonId);
            ViewBag.ActiveAvailability = "true";
            return View("Index", available);
        }

        // GET: /staff/filter — AJAX endpoint, returns partial HTML rows
        [AllowAnonymous]
        [HttpGet("staff/filter")]
        public IActionResult Filter(string? name, string? specialization, string? salonName, bool? available)
        {
            var staff = _staffRepo.Filter(name, specialization, salonName, available);
            return PartialView("_StaffTable", staff);
        }

        // GET: /Staff/Edit/5
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var staff = _staffRepo.GetById(id);
            if (staff == null) return NotFound();

            var vm = new EditStaffViewModel
            {
                Id = id,
                UserId = staff.UserId,
                Specialization = staff.Specialization,
                HourlyRate = staff.HourlyRate,
                IsAvailable = staff.IsAvailable,
                // Snapshot — stored as hidden fields, compared on POST
                OriginalUserId = staff.UserId,
                OriginalSpecialization = staff.Specialization,
                OriginalHourlyRate = staff.HourlyRate,
                OriginalIsAvailable = staff.IsAvailable,
                // Display
                StaffFullName = $"{staff.User?.FirstName} {staff.User?.LastName}",
                Users = _userRepo.GetStaffUsers()
            };
            return View(vm);
        }

        // POST: /Staff/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, EditStaffViewModel vm)
        {
            ModelState.Remove(nameof(EditStaffViewModel.Users));
            ModelState.Remove(nameof(EditStaffViewModel.StaffFullName));

            if (vm.UserId != vm.OriginalUserId)
            {
                var existingStaff = _staffRepo.GetByUserId(vm.UserId);
                if (existingStaff != null)
                    ModelState.AddModelError(nameof(EditStaffViewModel.UserId), "This user is already assigned as a staff member.");
            }

            if (!ModelState.IsValid)
            {
                vm.Users = _userRepo.GetStaffUsers();
                return View(vm);
            }

            var current = _staffRepo.GetById(id);
            if (current == null) return NotFound();

            // Optimistic concurrency: check that the DB still holds the values
            // the user saw when they opened the form
            bool conflict =
                current.UserId != vm.OriginalUserId ||
                current.Specialization != vm.OriginalSpecialization ||
                current.HourlyRate != vm.OriginalHourlyRate ||
                current.IsAvailable != vm.OriginalIsAvailable;

            if (conflict)
            {
                ModelState.AddModelError(string.Empty,
                    "This record was changed by someone else while you were editing. " +
                    $"The current values are now shown below " +
                    $"(e.g. Specialization is \"{current.Specialization}\", " +
                    $"Hourly Rate is {current.HourlyRate:C}). " +
                    "Review the current values and save again.");

                // Reset snapshot to current DB values so the next save uses the latest baseline
                vm.OriginalUserId = current.UserId;
                vm.OriginalSpecialization = current.Specialization;
                vm.OriginalHourlyRate = current.HourlyRate;
                vm.OriginalIsAvailable = current.IsAvailable;
                // Show current DB values in the form so the user sees what changed
                vm.UserId = current.UserId;
                vm.Specialization = current.Specialization;
                vm.HourlyRate = current.HourlyRate;
                vm.IsAvailable = current.IsAvailable;
                vm.StaffFullName = $"{current.User?.FirstName} {current.User?.LastName}";
                vm.Users = _userRepo.GetStaffUsers();
                return View(vm);
            }

            current.UserId = vm.UserId;
            current.Specialization = vm.Specialization;
            current.HourlyRate = vm.HourlyRate;
            current.IsAvailable = vm.IsAvailable;
            _staffRepo.Update(current);

            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _staffRepo.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
