using manage_my_hairsaloon.Models;

namespace manage_my_hairsaloon.Repositories
{
    public class MockReservationRepository
    {
        private readonly List<Reservation> _reservations = MockDataStore.Reservations;

        public List<Reservation> GetAll()
        {
            return _reservations;
        }

        public Reservation? GetById(int id)
        {
            return _reservations.FirstOrDefault(r => r.Id == id);
        }

        public List<Reservation> GetByCustomerId(int customerId)
        {
            return _reservations.Where(r => r.CustomerId == customerId).ToList();
        }

        public List<Reservation> GetByStaffId(int staffId)
        {
            return _reservations.Where(r => r.StaffId == staffId).ToList();
        }

        public List<Reservation> GetByStatus(ReservationStatus status)
        {
            return _reservations.Where(r => r.Status == status).ToList();
        }

        public List<Reservation> GetByServiceId(int serviceId)
        {
            return _reservations.Where(r => r.ServiceId == serviceId).ToList();
        }
    }
}
