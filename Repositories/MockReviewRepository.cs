using manage_my_hairsaloon.Models;

namespace manage_my_hairsaloon.Repositories
{
    public class MockReviewRepository
    {
        private readonly List<Review> _reviews = MockDataStore.Reviews;

        public List<Review> GetAll()
        {
            return _reviews;
        }

        public Review? GetById(int id)
        {
            return _reviews.FirstOrDefault(r => r.Id == id);
        }

        public List<Review> GetByReservationId(int reservationId)
        {
            return _reviews.Where(r => r.ReservationId == reservationId).ToList();
        }

        public List<Review> GetByCustomerId(int customerId)
        {
            return _reviews.Where(r => r.CustomerId == customerId).ToList();
        }
    }
}
