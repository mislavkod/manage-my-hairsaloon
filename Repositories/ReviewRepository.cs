using manage_my_hairsaloon.Data;
using manage_my_hairsaloon.Models;
using Microsoft.EntityFrameworkCore;

namespace manage_my_hairsaloon.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Review> GetAll()
        {
            return _context.Reviews
                .Where(r => r.DeletedAt == null)
                .Include(r => r.Customer)
                .Include(r => r.Reservation)
                .ToList();
        }

        public Review? GetById(int id)
        {
            return _context.Reviews
                .Where(r => r.DeletedAt == null)
                .Include(r => r.Customer)
                .Include(r => r.Reservation)
                .FirstOrDefault(r => r.Id == id);
        }

        public List<Review> GetByReservationId(int reservationId)
        {
            return _context.Reviews
                .Where(r => r.DeletedAt == null)
                .Include(r => r.Customer)
                .Where(r => r.ReservationId == reservationId)
                .ToList();
        }

        public List<Review> GetByCustomerId(int customerId)
        {
            return _context.Reviews
                .Where(r => r.DeletedAt == null)
                .Include(r => r.Reservation)
                .Where(r => r.CustomerId == customerId)
                .ToList();
        }

        public List<Review> GetBySalonId(int salonId)
        {
            return _context.Reviews
                .Where(r => r.DeletedAt == null)
                .Include(r => r.Customer)
                .Include(r => r.Reservation)
                    .ThenInclude(res => res!.Staff)
                .Where(r => r.Reservation != null &&
                            r.Reservation.Staff != null &&
                            r.Reservation.Staff.HairSalonId == salonId)
                .ToList();
        }

        public List<Review> Filter(string? customerName, int? minRating, string? comment)
        {
            var q = _context.Reviews
                .Where(r => r.DeletedAt == null)
                .Include(r => r.Customer)
                .Include(r => r.Reservation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(customerName))
            {
                var lower = customerName.ToLower();
                q = q.Where(r =>
                    (r.Customer != null && r.Customer.FirstName != null && r.Customer.FirstName.ToLower().Contains(lower)) ||
                    (r.Customer != null && r.Customer.LastName  != null && r.Customer.LastName.ToLower().Contains(lower)));
            }
            if (minRating.HasValue)
                q = q.Where(r => r.Rating >= minRating.Value);

            if (!string.IsNullOrWhiteSpace(comment))
            {
                var lower = comment.ToLower();
                q = q.Where(r => r.Comment != null && r.Comment.ToLower().Contains(lower));
            }

            return q.ToList();
        }

        public void Add(Review review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();
        }

        public void Update(Review review)
        {
            _context.Reviews.Update(review);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var review = _context.Reviews.Find(id);
            if (review != null)
            {
                review.DeletedAt = DateTime.UtcNow;
                _context.SaveChanges();
            }
        }
    }
}
