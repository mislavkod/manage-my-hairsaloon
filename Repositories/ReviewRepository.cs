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
                .Include(r => r.Customer)
                .Include(r => r.Reservation)
                .ToList();
        }

        public Review? GetById(int id)
        {
            return _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Reservation)
                .FirstOrDefault(r => r.Id == id);
        }

        public List<Review> GetByReservationId(int reservationId)
        {
            return _context.Reviews
                .Include(r => r.Customer)
                .Where(r => r.ReservationId == reservationId)
                .ToList();
        }

        public List<Review> GetByCustomerId(int customerId)
        {
            return _context.Reviews
                .Include(r => r.Reservation)
                .Where(r => r.CustomerId == customerId)
                .ToList();
        }
    }
}
