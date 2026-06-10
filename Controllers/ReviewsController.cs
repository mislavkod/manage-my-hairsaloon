using manage_my_hairsaloon.Models;
using manage_my_hairsaloon.Repositories;
using manage_my_hairsaloon.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace manage_my_hairsaloon.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly IReviewRepository _reviewRepo;
        private readonly IReservationRepository _reservationRepo;
        private readonly IUserRepository _userRepo;
        private readonly IStaffRepository _staffRepo;

        public ReviewsController(
            IReviewRepository reviewRepo,
            IReservationRepository reservationRepo,
            IUserRepository userRepo,
            IStaffRepository staffRepo)
        {
            _reviewRepo = reviewRepo;
            _reservationRepo = reservationRepo;
            _userRepo = userRepo;
            _staffRepo = staffRepo;
        }

        [Authorize]
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
                return View(_reviewRepo.GetAll());

            var email = User.FindFirstValue(ClaimTypes.Email);
            var businessUser = email != null ? _userRepo.GetByEmail(email) : null;
            if (businessUser == null)
                return View(new List<Review>());

            if (User.IsInRole("Staff"))
            {
                var staffRecord = _staffRepo.GetByUserId(businessUser.Id);
                if (staffRecord != null)
                    return View(_reviewRepo.GetBySalonId(staffRecord.HairSalonId));
                return View(new List<Review>());
            }

            // Customer sees reviews for their own reservations
            return View(_reviewRepo.GetByCustomerId(businessUser.Id));
        }

        [Authorize]
        public IActionResult Details(int id)
        {
            var review = _reviewRepo.GetById(id);
            if (review == null) return NotFound();
            return View(review);
        }

        // GET: /reviews/filter — AJAX endpoint, returns partial HTML rows
        [Authorize]
        [HttpGet("reviews/filter")]
        public IActionResult Filter(string? customerName, int? minRating, string? comment)
        {
            var reviews = _reviewRepo.Filter(customerName, minRating, comment);
            return PartialView("_ReviewsTable", reviews);
        }

        // GET: /reservations/{reservationId}/NewReview?salonId={salonId}
        [Authorize]
        [HttpGet("reservations/{reservationId}/NewReview")]
        public IActionResult NewReview(int reservationId, int salonId)
        {
            var reservation = _reservationRepo.GetById(reservationId);
            if (reservation == null) return NotFound();
            if (reservation.Status != ReservationStatus.Completed)
                return BadRequest("Reviews can only be added to completed reservations.");

            // TODO: When login is implemented, check reservation.CustomerId == GetCurrentUserId()

            var vm = new NewReviewViewModel
            {
                ReservationId = reservationId,
                SalonId = salonId,
                Reservation = reservation,
                Customers = _userRepo.GetCustomers()
            };
            return View(vm);
        }

        // POST: /reservations/{reservationId}/NewReview
        [Authorize]
        [HttpPost("reservations/{reservationId}/NewReview")]
        [ValidateAntiForgeryToken]
        public IActionResult NewReview(int reservationId, NewReviewViewModel vm)
        {
            var reservation = _reservationRepo.GetById(reservationId);
            if (reservation == null) return NotFound();
            if (reservation.Status != ReservationStatus.Completed)
                return BadRequest("Reviews can only be added to completed reservations.");

            // TODO: When login is implemented, check reservation.CustomerId == GetCurrentUserId()

            ModelState.Remove(nameof(NewReviewViewModel.Reservation));
            ModelState.Remove(nameof(NewReviewViewModel.Customers));

            if (_reviewRepo.GetByReservationId(reservationId).Any())
                ModelState.AddModelError(string.Empty, "A review already exists for this reservation.");

            if (!ModelState.IsValid)
            {
                vm.Reservation = reservation;
                vm.Customers = _userRepo.GetCustomers();
                return View(vm);
            }

            var review = new Review
            {
                ReservationId = reservationId,
                CustomerId = vm.CustomerId,
                Rating = vm.Rating,
                Comment = vm.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _reviewRepo.Add(review);
            return RedirectToAction("Details", "HairSalons", new { id = vm.SalonId });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var review = _reviewRepo.GetById(id);
            if (review == null) return NotFound();

            var vm = new EditReviewViewModel
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                OriginalRating = review.Rating,
                OriginalComment = review.Comment,
                CustomerName = review.Customer != null
                    ? $"{review.Customer.FirstName} {review.Customer.LastName}"
                    : null,
                CreatedAt = review.CreatedAt
            };
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, EditReviewViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var current = _reviewRepo.GetById(id);
            if (current == null) return NotFound();

            bool conflict =
                current.Rating != vm.OriginalRating ||
                current.Comment != vm.OriginalComment;

            if (conflict)
            {
                ModelState.AddModelError(string.Empty,
                    "This record was modified by someone else. The current values are shown below. Please review and resubmit.");
                vm.OriginalRating = current.Rating;
                vm.OriginalComment = current.Comment;
                vm.Rating = current.Rating;
                vm.Comment = current.Comment;
                vm.CustomerName = current.Customer != null
                    ? $"{current.Customer.FirstName} {current.Customer.LastName}"
                    : null;
                vm.CreatedAt = current.CreatedAt;
                return View(vm);
            }

            current.Rating = vm.Rating;
            current.Comment = vm.Comment;
            _reviewRepo.Update(current);

            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _reviewRepo.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
