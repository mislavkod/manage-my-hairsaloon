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
    }
}
