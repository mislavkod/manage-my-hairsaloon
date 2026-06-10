using manage_my_hairsaloon.Models;
using manage_my_hairsaloon.Repositories;
using manage_my_hairsaloon.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace manage_my_hairsaloon.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepo;

        public UsersController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var users = _userRepo.GetAll();
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Details(int id)
        {
            var user = _userRepo.GetById(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // GET: /users/filter — AJAX endpoint, returns partial HTML rows
        [Authorize(Roles = "Admin")]
        [HttpGet("users/filter")]
        public IActionResult Filter(string? name, string? role, DateTime? createdBefore)
        {
            var users = _userRepo.Filter(name, role, createdBefore);
            return PartialView("_UsersTable", users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _userRepo.GetById(id);
            if (user == null) return NotFound();

            var vm = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                OriginalEmail = user.Email,
                OriginalFirstName = user.FirstName,
                OriginalLastName = user.LastName,
                OriginalPhoneNumber = user.PhoneNumber,
                OriginalRole = user.Role
            };
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, EditUserViewModel vm)
        {
            if (!string.Equals(vm.Email, vm.OriginalEmail, StringComparison.OrdinalIgnoreCase))
            {
                var existing = _userRepo.GetByEmail(vm.Email!);
                if (existing != null && existing.Id != id)
                    ModelState.AddModelError(nameof(EditUserViewModel.Email), "This email address is already in use by another account.");
            }

            if (!ModelState.IsValid)
                return View(vm);

            var current = _userRepo.GetById(id);
            if (current == null) return NotFound();

            bool conflict =
                current.Email != vm.OriginalEmail ||
                current.FirstName != vm.OriginalFirstName ||
                current.LastName != vm.OriginalLastName ||
                current.PhoneNumber != vm.OriginalPhoneNumber ||
                current.Role != vm.OriginalRole;

            if (conflict)
            {
                ModelState.AddModelError(string.Empty,
                    "This record was modified by someone else. The current values are shown below. Please review and resubmit.");
                vm.OriginalEmail = current.Email;
                vm.OriginalFirstName = current.FirstName;
                vm.OriginalLastName = current.LastName;
                vm.OriginalPhoneNumber = current.PhoneNumber;
                vm.OriginalRole = current.Role;
                vm.Email = current.Email;
                vm.FirstName = current.FirstName;
                vm.LastName = current.LastName;
                vm.PhoneNumber = current.PhoneNumber;
                vm.Role = current.Role;
                return View(vm);
            }

            current.Email = vm.Email;
            current.FirstName = vm.FirstName;
            current.LastName = vm.LastName;
            current.PhoneNumber = vm.PhoneNumber;
            current.Role = vm.Role;
            _userRepo.Update(current);

            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _userRepo.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
