using manage_my_hairsaloon.Models;

namespace manage_my_hairsaloon.Repositories
{
    public interface IStaffRepository
    {
        List<Staff> GetAll();
        Staff? GetById(int id);
        List<Staff> GetBySalonId(int salonId);
        Staff? GetByUserId(int userId);
        List<Staff> GetAvailable();
        List<Staff> GetAvailableBySalonId(int salonId);
    }
}
