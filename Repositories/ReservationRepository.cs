using manage_my_hairsaloon.Data;
using manage_my_hairsaloon.Models;
using Microsoft.EntityFrameworkCore;

namespace manage_my_hairsaloon.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext _context;

        public ReservationRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Reservation> GetAll()
        {
            return _context.Reservations
                .Where(r => r.DeletedAt == null)
                .Include(r => r.Customer)
                .Include(r => r.Staff)
                .Include(r => r.Service)
                .ToList();
        }

        public Reservation? GetById(int id)
        {
            return _context.Reservations
                .Where(r => r.DeletedAt == null)
                .Include(r => r.Customer)
                .Include(r => r.Staff)
                .Include(r => r.Service)
                .Include(r => r.Reviews)
                .FirstOrDefault(r => r.Id == id);
        }

        public List<Reservation> GetByCustomerId(int customerId)
        {
            return _context.Reservations
                .Where(r => r.DeletedAt == null)
                .Include(r => r.Staff)
                .Include(r => r.Service)
                .Where(r => r.CustomerId == customerId)
                .ToList();
        }

        public List<Reservation> GetByStaffId(int staffId)
        {
            return _context.Reservations
                .Where(r => r.DeletedAt == null)
                .Include(r => r.Customer)
                .Include(r => r.Service)
                .Where(r => r.StaffId == staffId)
                .ToList();
        }

        public List<Reservation> GetByStatus(ReservationStatus status)
        {
            return _context.Reservations
                .Where(r => r.DeletedAt == null)
                .Include(r => r.Customer)
                .Include(r => r.Staff)
                .Include(r => r.Service)
                .Where(r => r.Status == status)
                .ToList();
        }

        public List<Reservation> GetByServiceId(int serviceId)
        {
            return _context.Reservations
                .Where(r => r.DeletedAt == null)
                .Include(r => r.Customer)
                .Include(r => r.Staff)
                .Where(r => r.ServiceId == serviceId)
                .ToList();
        }

        public List<Reservation> GetBySalonIdAndStatus(int salonId, ReservationStatus status)
        {
            return _context.Reservations
                .Where(r => r.DeletedAt == null)
                .Include(r => r.Customer)
                .Include(r => r.Staff)
                .Include(r => r.Service)
                .Where(r => r.Staff!.HairSalonId == salonId && r.Status == status)
                .ToList();
        }

        public List<Reservation> GetBySalonIdAndDate(int salonId, DateTime date)
        {
            return _context.Reservations
                .Where(r => r.DeletedAt == null)
                .Include(r => r.Staff)
                .Where(r => r.Staff!.HairSalonId == salonId
                    && r.ReservationDateTime.Date == date.Date
                    && r.Status != ReservationStatus.Cancelled)
                .ToList();
        }

        public List<Reservation> Filter(string? status, string? customerName, string? serviceName, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = _context.Reservations
                .Where(r => r.DeletedAt == null)
                .Include(r => r.Customer)
                .Include(r => r.Staff)
                .Include(r => r.Service)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<ReservationStatus>(status, out var parsedStatus))
                query = query.Where(r => r.Status == parsedStatus);

            if (!string.IsNullOrWhiteSpace(customerName))
                query = query.Where(r =>
                    r.Customer != null &&
                    (r.Customer.FirstName.Contains(customerName) ||
                     r.Customer.LastName.Contains(customerName)));

            if (!string.IsNullOrWhiteSpace(serviceName))
                query = query.Where(r =>
                    r.Service != null && r.Service.Name.Contains(serviceName));

            if (dateFrom.HasValue)
                query = query.Where(r => r.ReservationDateTime >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(r => r.ReservationDateTime <= dateTo.Value);

            return query.ToList();
        }

        public void Add(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var reservation = _context.Reservations.Find(id);
            if (reservation != null)
            {
                reservation.DeletedAt = DateTime.UtcNow;
                _context.SaveChanges();
            }
        }
    }
}
