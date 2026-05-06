using manage_my_hairsaloon.Data;
using manage_my_hairsaloon.Models;
using Microsoft.EntityFrameworkCore;

namespace manage_my_hairsaloon.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly AppDbContext _context;

        public ServiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Service> GetAll()
        {
            return _context.Services
                .Include(s => s.HairSalon)
                .ToList();
        }

        public Service? GetById(int id)
        {
            return _context.Services
                .Include(s => s.HairSalon)
                .FirstOrDefault(s => s.Id == id);
        }

        public List<Service> GetBySalonId(int salonId)
        {
            return _context.Services
                .Include(s => s.HairSalon)
                .Where(s => s.HairSalonId == salonId)
                .ToList();
        }

        public List<Service> GetByCategory(ServiceCategory category)
        {
            return _context.Services
                .Include(s => s.HairSalon)
                .Where(s => s.Category == category)
                .ToList();
        }

        public List<Service> GetByMaxPrice(decimal maxPrice)
        {
            return _context.Services
                .Include(s => s.HairSalon)
                .Where(s => s.Price <= maxPrice)
                .ToList();
        }
    }
}
