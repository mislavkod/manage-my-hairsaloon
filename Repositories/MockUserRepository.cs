using manage_my_hairsaloon.Models;

namespace manage_my_hairsaloon.Repositories
{
    public class MockUserRepository
    {
        private readonly List<User> _users = MockDataStore.Users;

        public List<User> GetAll()
        {
            return _users;
        }

        public User? GetById(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public List<User> GetByRole(UserRole role)
        {
            return _users.Where(u => u.Role == role).ToList();
        }

        public List<User> GetCustomers()
        {
            return GetByRole(UserRole.Customer);
        }

        public List<User> GetStaffUsers()
        {
            return GetByRole(UserRole.Staff);
        }

        public User? GetByEmail(string email)
        {
            return _users.FirstOrDefault(u => u.Email == email);
        }
    }
}
