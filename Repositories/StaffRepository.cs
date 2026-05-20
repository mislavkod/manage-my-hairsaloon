using manage_my_hairsaloon.Data;
using manage_my_hairsaloon.Models;
using Microsoft.EntityFrameworkCore;

namespace manage_my_hairsaloon.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly AppDbContext _context;

        public StaffRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Staff> GetAll()
        {
            return _context.Staff
                .Where(s => s.DeletedAt == null)
                .Include(s => s.User)
                .Include(s => s.HairSalon)
                .ToList();
        }

        public Staff? GetById(int id)
        {
            return _context.Staff
                .Where(s => s.DeletedAt == null)
                .Include(s => s.User)
                .Include(s => s.HairSalon)
                .FirstOrDefault(s => s.Id == id);
        }

        public List<Staff> GetBySalonId(int salonId)
        {
            return _context.Staff
                .Where(s => s.DeletedAt == null)
                .Include(s => s.User)
                .Where(s => s.HairSalonId == salonId)
                .ToList();
        }

        public Staff? GetByUserId(int userId)
        {
            return _context.Staff
                .Where(s => s.DeletedAt == null)
                .Include(s => s.HairSalon)
                .FirstOrDefault(s => s.UserId == userId);
        }

        public List<Staff> GetAvailable()
        {
            return _context.Staff
                .Where(s => s.DeletedAt == null)
                .Include(s => s.User)
                .Include(s => s.HairSalon)
                .Where(s => s.IsAvailable)
                .ToList();
        }

        public List<Staff> GetAvailableBySalonId(int salonId)
        {
            return _context.Staff
                .Where(s => s.DeletedAt == null)
                .Include(s => s.User)
                .Include(s => s.HairSalon)
                .Where(s => s.HairSalonId == salonId && s.IsAvailable)
                .ToList();
        }

        public List<Staff> Filter(string? name, string? specialization, string? salonName, bool? available)
        {
            var q = _context.Staff
                .Where(s => s.DeletedAt == null)
                .Include(s => s.User)
                .Include(s => s.HairSalon)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var lower = name.ToLower();
                q = q.Where(s =>
                    (s.User != null && s.User.FirstName != null && s.User.FirstName.ToLower().Contains(lower)) ||
                    (s.User != null && s.User.LastName  != null && s.User.LastName.ToLower().Contains(lower)));
            }
            if (!string.IsNullOrWhiteSpace(specialization))
            {
                var lower = specialization.ToLower();
                q = q.Where(s => s.Specialization != null && s.Specialization.ToLower().Contains(lower));
            }
            if (!string.IsNullOrWhiteSpace(salonName))
            {
                var lower = salonName.ToLower();
                q = q.Where(s => s.HairSalon != null && s.HairSalon.Name != null && s.HairSalon.Name.ToLower().Contains(lower));
            }
            if (available.HasValue)
                q = q.Where(s => s.IsAvailable == available.Value);

            return q.ToList();
        }

        public void Add(Staff staff)
        {
            _context.Staff.Add(staff);
            _context.SaveChanges();
        }

        public void Update(Staff staff)
        {
            _context.Staff.Update(staff);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var staff = _context.Staff.Find(id);
            if (staff != null)
            {
                staff.DeletedAt = DateTime.UtcNow;
                _context.SaveChanges();
            }
        }
    }
}
