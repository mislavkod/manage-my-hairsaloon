using manage_my_hairsaloon.DTOs;
using manage_my_hairsaloon.Models;
using manage_my_hairsaloon.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace manage_my_hairsaloon.Controllers
{
    [Route("api/hairsalons")]
    [ApiController]
    public class HairSalonsApiController : ControllerBase
    {
        private readonly IHairSalonRepository _repository;

        public HairSalonsApiController(IHairSalonRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<HairSalonDTO>> Get([FromQuery] string? q)
        {
            var salons = _repository.GetAll();

            if (!string.IsNullOrWhiteSpace(q))
            {
                salons = salons
                    .Where(s => s.Name != null && s.Name.Contains(q, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return Ok(salons.Select(ToDTO));
        }

        [HttpGet("{id}")]
        public ActionResult<HairSalonDTO> Get(int id)
        {
            var salon = _repository.GetById(id);

            if (salon == null)
                return NotFound();

            return Ok(ToDTO(salon));
        }

        [HttpPost]
        public ActionResult<HairSalonDTO> Post([FromBody] HairSalonRequest model)
        {
            var salon = new HairSalon
            {
                Name = model.Name,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                CreatedAt = DateTime.UtcNow
            };

            _repository.Add(salon);

            return CreatedAtAction(nameof(Get), new { id = salon.Id }, ToDTO(salon));
        }

        [HttpPut("{id}")]
        public ActionResult<HairSalonDTO> Put(int id, [FromBody] HairSalonRequest model)
        {
            var salon = _repository.GetById(id);

            if (salon == null)
                return NotFound();

            salon.Name = model.Name;
            salon.Address = model.Address;
            salon.PhoneNumber = model.PhoneNumber;
            salon.Email = model.Email;

            _repository.Update(salon);

            return Ok(ToDTO(salon));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var salon = _repository.GetById(id);

            if (salon == null)
                return NotFound();

            _repository.Delete(id);

            return NoContent();
        }

        private static HairSalonDTO ToDTO(HairSalon salon) => new()
        {
            Id = salon.Id,
            Name = salon.Name,
            Address = salon.Address,
            PhoneNumber = salon.PhoneNumber,
            Email = salon.Email
        };
    }
}
