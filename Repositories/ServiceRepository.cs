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
                .Where(s => s.DeletedAt == null)
                .Include(s => s.HairSalon)
                .ToList();
        }

        public Service? GetById(int id)
        {
            return _context.Services
                .Where(s => s.DeletedAt == null)
                .Include(s => s.HairSalon)
                .FirstOrDefault(s => s.Id == id);
        }

        public List<Service> GetBySalonId(int salonId)
        {
            return _context.Services
                .Where(s => s.DeletedAt == null)
                .Include(s => s.HairSalon)
                .Where(s => s.HairSalonId == salonId)
                .ToList();
        }

        public List<Service> GetByCategory(ServiceCategory category)
        {
            return _context.Services
                .Where(s => s.DeletedAt == null)
                .Include(s => s.HairSalon)
                .Where(s => s.Category == category)
                .ToList();
        }

        public List<Service> GetByMaxPrice(decimal maxPrice)
        {
            return _context.Services
                .Where(s => s.DeletedAt == null)
                .Include(s => s.HairSalon)
                .Where(s => s.Price <= maxPrice)
                .ToList();
        }

        public List<Service> Filter(string? category, string? serviceName, string? salonName, decimal? maxPrice)
        {
            var query = _context.Services
                .Where(s => s.DeletedAt == null)
                .Include(s => s.HairSalon)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(category) &&
                Enum.TryParse<ServiceCategory>(category, out var cat))
                query = query.Where(s => s.Category == cat);

            if (!string.IsNullOrWhiteSpace(serviceName))
                query = query.Where(s => s.Name.Contains(serviceName));

            if (!string.IsNullOrWhiteSpace(salonName))
                query = query.Where(s => s.HairSalon != null && s.HairSalon.Name.Contains(salonName));

            if (maxPrice.HasValue)
                query = query.Where(s => s.Price <= maxPrice.Value);

            return query.ToList();
        }

        public decimal GetMaxPrice()
        {
            return _context.Services
                .Where(s => s.DeletedAt == null)
                .Max(s => (decimal?)s.Price) ?? 0m;
        }

        public void Add(Service service)
        {
            _context.Services.Add(service);
            _context.SaveChanges();
        }

        public void Update(Service service)
        {
            _context.Services.Update(service);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var service = _context.Services.Find(id);
            if (service != null)
            {
                service.DeletedAt = DateTime.UtcNow;
                _context.SaveChanges();
            }
        }
    }
}
