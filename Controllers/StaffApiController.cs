using manage_my_hairsaloon.DTOs;
using manage_my_hairsaloon.Models;
using manage_my_hairsaloon.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace manage_my_hairsaloon.Controllers
{
    [Route("api/staff")]
    [ApiController]
    public class StaffApiController : ControllerBase
    {
        private readonly IStaffRepository _repository;

        public StaffApiController(IStaffRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<StaffDTO>> Get(
            [FromQuery] string? name,
            [FromQuery] string? specialization,
            [FromQuery] string? salonName,
            [FromQuery] bool? available)
        {
            var staff = _repository.Filter(name, specialization, salonName, available);
            return Ok(staff.Select(ToDTO));
        }

        [HttpGet("{id}")]
        public ActionResult<StaffDTO> Get(int id)
        {
            var staff = _repository.GetById(id);

            if (staff == null)
                return NotFound();

            return Ok(ToDTO(staff));
        }

        [HttpPost]
        public ActionResult<StaffDTO> Post([FromBody] StaffRequest model)
        {
            var staff = new Staff
            {
                UserId = model.UserId,
                HairSalonId = model.HairSalonId,
                Specialization = model.Specialization,
                HourlyRate = model.HourlyRate,
                IsAvailable = model.IsAvailable
            };

            _repository.Add(staff);

            return CreatedAtAction(nameof(Get), new { id = staff.Id }, ToDTO(staff));
        }

        [HttpPut("{id}")]
        public ActionResult<StaffDTO> Put(int id, [FromBody] StaffRequest model)
        {
            var staff = _repository.GetById(id);

            if (staff == null)
                return NotFound();

            staff.UserId = model.UserId;
            staff.HairSalonId = model.HairSalonId;
            staff.Specialization = model.Specialization;
            staff.HourlyRate = model.HourlyRate;
            staff.IsAvailable = model.IsAvailable;

            _repository.Update(staff);

            return Ok(ToDTO(staff));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var staff = _repository.GetById(id);

            if (staff == null)
                return NotFound();

            _repository.Delete(id);

            return NoContent();
        }

        private static StaffDTO ToDTO(Staff staff) => new()
        {
            Id = staff.Id,
            Specialization = staff.Specialization,
            HourlyRate = staff.HourlyRate,
            IsAvailable = staff.IsAvailable,
            HairSalonName = staff.HairSalon?.Name,
            User = staff.User == null ? null : new UserDTO
            {
                Id = staff.User.Id,
                Email = staff.User.Email,
                FirstName = staff.User.FirstName,
                LastName = staff.User.LastName,
                PhoneNumber = staff.User.PhoneNumber,
                Role = staff.User.Role.ToString()
            }
        };
    }
}
