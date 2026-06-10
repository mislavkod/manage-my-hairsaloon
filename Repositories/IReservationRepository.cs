using manage_my_hairsaloon.Models;

namespace manage_my_hairsaloon.Repositories
{
    public interface IReservationRepository
    {
        List<Reservation> GetAll();
        Reservation? GetById(int id);
        List<Reservation> GetByCustomerId(int customerId);
        List<Reservation> GetByStaffId(int staffId);
        List<Reservation> GetBySalonId(int salonId);
        List<Reservation> GetByStatus(ReservationStatus status);
        List<Reservation> GetByServiceId(int serviceId);
        List<Reservation> GetBySalonIdAndStatus(int salonId, ReservationStatus status);
        List<Reservation> GetBySalonIdAndDate(int salonId, DateTime date);
        List<Reservation> Filter(string? status, string? customerName, string? serviceName, DateTime? dateFrom, DateTime? dateTo);
        void Add(Reservation reservation);
        void Delete(int id);
    }
}
