using manage_my_hairsaloon.Models;

namespace manage_my_hairsaloon.Repositories
{
    public interface IReservationRepository
    {
        List<Reservation> GetAll();
        Reservation? GetById(int id);
        List<Reservation> GetByCustomerId(int customerId);
        List<Reservation> GetByStaffId(int staffId);
        List<Reservation> GetByStatus(ReservationStatus status);
        List<Reservation> GetByServiceId(int serviceId);
        List<Reservation> GetBySalonIdAndStatus(int salonId, ReservationStatus status);
        List<Reservation> GetBySalonIdAndDate(int salonId, DateTime date);
        void Add(Reservation reservation);
    }
}
