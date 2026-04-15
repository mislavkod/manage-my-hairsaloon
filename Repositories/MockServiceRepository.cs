using manage_my_hairsaloon.Models;

namespace manage_my_hairsaloon.Repositories
{
    public class MockServiceRepository
    {
        private readonly List<Service> _services = MockDataStore.Services;

        public List<Service> GetAll()
        {
            return _services;
        }

        public Service? GetById(int id)
        {
            return _services.FirstOrDefault(s => s.Id == id);
        }

        public List<Service> GetBySalonId(int salonId)
        {
            return _services.Where(s => s.HairSalonId == salonId).ToList();
        }

        public List<Service> GetByCategory(ServiceCategory category)
        {
            return _services.Where(s => s.Category == category).ToList();
        }

        public List<Service> GetByMaxPrice(decimal maxPrice)
        {
            return _services.Where(s => s.Price <= maxPrice).ToList();
        }
    }
}
