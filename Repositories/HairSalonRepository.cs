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
                .Include(h => h.Staff)
                .Include(h => h.Services)
                .ToList();
        }

        public HairSalon? GetById(int id)
        {
            return _context.HairSalons
                .Include(h => h.Staff)
                .Include(h => h.Services)
                .FirstOrDefault(h => h.Id == id);
        }

        public HairSalon? GetByName(string name)
        {
            return _context.HairSalons
                .Include(h => h.Staff)
                .Include(h => h.Services)
                .FirstOrDefault(h => h.Name == name);
        }
    }
}
