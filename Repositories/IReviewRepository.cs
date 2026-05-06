using manage_my_hairsaloon.Models;

namespace manage_my_hairsaloon.Repositories
{
    public interface IReviewRepository
    {
        List<Review> GetAll();
        Review? GetById(int id);
        List<Review> GetByReservationId(int reservationId);
        List<Review> GetByCustomerId(int customerId);
    }
}
