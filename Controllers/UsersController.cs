using manage_my_hairsaloon.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace manage_my_hairsaloon.Controllers
{
    public class UsersController : Controller
    {
        private readonly MockUserRepository _userRepo;

        public UsersController(MockUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public IActionResult Index()
        {
            var users = _userRepo.GetAll();
            return View(users);
        }

        public IActionResult Details(int id)
        {
            var user = _userRepo.GetById(id);
            if (user == null) return NotFound();
            return View(user);
        }
    }
}
