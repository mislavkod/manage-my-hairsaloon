using manage_my_hairsaloon.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace manage_my_hairsaloon.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly IReviewRepository _reviewRepo;

        public ReviewsController(IReviewRepository reviewRepo)
        {
            _reviewRepo = reviewRepo;
        }

        public IActionResult Index()
        {
            var reviews = _reviewRepo.GetAll();
            return View(reviews);
        }

        public IActionResult Details(int id)
        {
            var review = _reviewRepo.GetById(id);
            if (review == null) return NotFound();
            return View(review);
        }
    }
}
