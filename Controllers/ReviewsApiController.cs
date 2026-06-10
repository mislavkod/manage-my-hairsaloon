using manage_my_hairsaloon.DTOs;
using manage_my_hairsaloon.Models;
using manage_my_hairsaloon.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace manage_my_hairsaloon.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewsApiController : ControllerBase
    {
        private readonly IReviewRepository _repository;

        public ReviewsApiController(IReviewRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ReviewDTO>> Get(
            [FromQuery] string? customerName,
            [FromQuery] int? minRating,
            [FromQuery] string? comment)
        {
            var reviews = _repository.Filter(customerName, minRating, comment);
            return Ok(reviews.Select(ToDTO));
        }

        [HttpGet("{id}")]
        public ActionResult<ReviewDTO> Get(int id)
        {
            var review = _repository.GetById(id);

            if (review == null)
                return NotFound();

            return Ok(ToDTO(review));
        }

        [HttpPost]
        public ActionResult<ReviewDTO> Post([FromBody] ReviewRequest model)
        {
            var review = new Review
            {
                ReservationId = model.ReservationId,
                CustomerId = model.CustomerId,
                Rating = model.Rating,
                Comment = model.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _repository.Add(review);

            return CreatedAtAction(nameof(Get), new { id = review.Id }, ToDTO(review));
        }

        [HttpPut("{id}")]
        public ActionResult<ReviewDTO> Put(int id, [FromBody] ReviewRequest model)
        {
            var review = _repository.GetById(id);

            if (review == null)
                return NotFound();

            review.Rating = model.Rating;
            review.Comment = model.Comment;

            _repository.Update(review);

            return Ok(ToDTO(review));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var review = _repository.GetById(id);

            if (review == null)
                return NotFound();

            _repository.Delete(id);

            return NoContent();
        }

        private static ReviewDTO ToDTO(Review review) => new()
        {
            Id = review.Id,
            ReservationId = review.ReservationId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            Customer = review.Customer == null ? null : new UserDTO
            {
                Id = review.Customer.Id,
                Email = review.Customer.Email,
                FirstName = review.Customer.FirstName,
                LastName = review.Customer.LastName,
                PhoneNumber = review.Customer.PhoneNumber,
                Role = review.Customer.Role.ToString()
            }
        };
    }
}
