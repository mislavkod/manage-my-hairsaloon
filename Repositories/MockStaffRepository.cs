using manage_my_hairsaloon.Models;

namespace manage_my_hairsaloon.Repositories
{
    public class MockStaffRepository
    {
        private readonly List<Staff> _staff = MockDataStore.Staff;

        public List<Staff> GetAll()
        {
            return _staff;
        }

        public Staff? GetById(int id)
        {
            return _staff.FirstOrDefault(s => s.Id == id);
        }

        public List<Staff> GetBySalonId(int salonId)
        {
            return _staff.Where(s => s.HairSalonId == salonId).ToList();
        }

        public Staff? GetByUserId(int userId)
        {
            return _staff.FirstOrDefault(s => s.UserId == userId);
        }

        public List<Staff> GetAvailable()
        {
            return _staff.Where(s => s.IsAvailable).ToList();
        }
    }
}
