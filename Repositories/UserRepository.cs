using manage_my_hairsaloon.Data;
using manage_my_hairsaloon.Models;
using Microsoft.EntityFrameworkCore;

namespace manage_my_hairsaloon.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<User> GetAll()
        {
            return _context.Users.Where(u => u.DeletedAt == null).ToList();
        }

        public User? GetById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id && u.DeletedAt == null);
        }

        public List<User> GetByRole(UserRole role)
        {
            return _context.Users.Where(u => u.Role == role && u.DeletedAt == null).ToList();
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
            return _context.Users.FirstOrDefault(u => u.Email == email && u.DeletedAt == null);
        }

        public List<User> Filter(string? name, string? role, DateTime? createdBefore)
        {
            var q = _context.Users.Where(u => u.DeletedAt == null).AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var lower = name.ToLower();
                q = q.Where(u =>
                    (u.FirstName != null && u.FirstName.ToLower().Contains(lower)) ||
                    (u.LastName  != null && u.LastName.ToLower().Contains(lower)));
            }
            if (!string.IsNullOrWhiteSpace(role) && Enum.TryParse<UserRole>(role, out var parsed))
                q = q.Where(u => u.Role == parsed);

            if (createdBefore.HasValue)
                q = q.Where(u => u.CreatedAt < createdBefore.Value);

            return q.ToList();
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                user.DeletedAt = DateTime.UtcNow;
                _context.SaveChanges();
            }
        }
    }
}
