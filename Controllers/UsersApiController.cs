using manage_my_hairsaloon.DTOs;
using manage_my_hairsaloon.Models;
using manage_my_hairsaloon.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace manage_my_hairsaloon.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersApiController : ControllerBase
    {
        private readonly IUserRepository _repository;

        public UsersApiController(IUserRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserDTO>> Get(
            [FromQuery] string? name,
            [FromQuery] string? role,
            [FromQuery] DateTime? createdBefore)
        {
            var users = _repository.Filter(name, role, createdBefore);
            return Ok(users.Select(ToDTO));
        }

        [HttpGet("{id}")]
        public ActionResult<UserDTO> Get(int id)
        {
            var user = _repository.GetById(id);

            if (user == null)
                return NotFound();

            return Ok(ToDTO(user));
        }

        [HttpPost]
        public ActionResult<UserDTO> Post([FromBody] UserRequest model)
        {
            var user = new User
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Role = model.Role,
                CreatedAt = DateTime.UtcNow
            };

            _repository.Add(user);

            return CreatedAtAction(nameof(Get), new { id = user.Id }, ToDTO(user));
        }

        [HttpPut("{id}")]
        public ActionResult<UserDTO> Put(int id, [FromBody] UserRequest model)
        {
            var user = _repository.GetById(id);

            if (user == null)
                return NotFound();

            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.Role = model.Role;

            _repository.Update(user);

            return Ok(ToDTO(user));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = _repository.GetById(id);

            if (user == null)
                return NotFound();

            _repository.Delete(id);

            return NoContent();
        }

        private static UserDTO ToDTO(User user) => new()
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.ToString()
        };
    }
}
