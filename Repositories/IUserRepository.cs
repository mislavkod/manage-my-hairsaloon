using manage_my_hairsaloon.Models;

namespace manage_my_hairsaloon.Repositories
{
    public interface IUserRepository
    {
        List<User> GetAll();
        User? GetById(int id);
        List<User> GetByRole(UserRole role);
        List<User> GetCustomers();
        List<User> GetStaffUsers();
        User? GetByEmail(string email);
        List<User> Filter(string? name, string? role, DateTime? createdBefore);
        void Add(User user);
        void Update(User user);
        void Delete(int id);
    }
}
