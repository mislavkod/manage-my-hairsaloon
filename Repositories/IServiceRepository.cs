using manage_my_hairsaloon.Models;

namespace manage_my_hairsaloon.Repositories
{
    public interface IServiceRepository
    {
        List<Service> GetAll();
        Service? GetById(int id);
        List<Service> GetBySalonId(int salonId);
        List<Service> GetByCategory(ServiceCategory category);
        List<Service> GetByMaxPrice(decimal maxPrice);
        List<Service> Filter(string? category, string? serviceName, string? salonName, decimal? maxPrice);
        decimal GetMaxPrice();
        void Add(Service service);
        void Update(Service service);
        void Delete(int id);
    }
}
