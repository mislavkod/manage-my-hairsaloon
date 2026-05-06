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
                .Include(s => s.User)
                .Include(s => s.HairSalon)
                .ToList();
        }

        public Staff? GetById(int id)
        {
            return _context.Staff
                .Include(s => s.User)
                .Include(s => s.HairSalon)
                .FirstOrDefault(s => s.Id == id);
        }

        public List<Staff> GetBySalonId(int salonId)
        {
            return _context.Staff
                .Include(s => s.User)
                .Where(s => s.HairSalonId == salonId)
                .ToList();
        }

        public Staff? GetByUserId(int userId)
        {
            return _context.Staff
                .Include(s => s.HairSalon)
                .FirstOrDefault(s => s.UserId == userId);
        }

        public List<Staff> GetAvailable()
        {
            return _context.Staff
                .Include(s => s.User)
                .Include(s => s.HairSalon)
                .Where(s => s.IsAvailable)
                .ToList();
        }

        public List<Staff> GetAvailableBySalonId(int salonId)
        {
            return _context.Staff
                .Include(s => s.User)
                .Include(s => s.HairSalon)
                .Where(s => s.HairSalonId == salonId && s.IsAvailable)
                .ToList();
        }
    }
}
