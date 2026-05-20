using manage_my_hairsaloon.Data;
using manage_my_hairsaloon.Models;
using Microsoft.EntityFrameworkCore;

namespace manage_my_hairsaloon.Repositories
{
    public class HairSalonRepository : IHairSalonRepository
    {
        private readonly AppDbContext _context;

        public HairSalonRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<HairSalon> GetAll()
        {
            return _context.HairSalons
                .Where(h => h.DeletedAt == null)
                .Include(h => h.Staff)
                .Include(h => h.Services)
                .ToList();
        }

        public HairSalon? GetById(int id)
        {
            return _context.HairSalons
                .Include(h => h.Staff)
                    .ThenInclude(s => s.User)
                .Include(h => h.Staff)
                    .ThenInclude(s => s.Reservations)
                        .ThenInclude(r => r.Customer)
                .Include(h => h.Staff)
                    .ThenInclude(s => s.Reservations)
                        .ThenInclude(r => r.Service)
                .Include(h => h.Staff)
                    .ThenInclude(s => s.Reservations)
                        .ThenInclude(r => r.Reviews)
                            .ThenInclude(rv => rv.Customer)
                .Include(h => h.Services)
                .FirstOrDefault(h => h.Id == id && h.DeletedAt == null);
        }

        public HairSalon? GetByName(string name)
        {
            return _context.HairSalons
                .Include(h => h.Staff)
                    .ThenInclude(s => s.User)
                .Include(h => h.Staff)
                    .ThenInclude(s => s.Reservations)
                        .ThenInclude(r => r.Customer)
                .Include(h => h.Staff)
                    .ThenInclude(s => s.Reservations)
                        .ThenInclude(r => r.Service)
                .Include(h => h.Staff)
                    .ThenInclude(s => s.Reservations)
                        .ThenInclude(r => r.Reviews)
                            .ThenInclude(rv => rv.Customer)
                .Include(h => h.Services)
                .FirstOrDefault(h => h.Name == name && h.DeletedAt == null);
        }

        public void Add(HairSalon salon)
        {
            _context.HairSalons.Add(salon);
            _context.SaveChanges();
        }

        public void Update(HairSalon salon)
        {
            _context.HairSalons.Update(salon);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var salon = _context.HairSalons.Find(id);
            if (salon != null)
            {
                salon.DeletedAt = DateTime.UtcNow;
                _context.SaveChanges();
            }
        }
    }
}
