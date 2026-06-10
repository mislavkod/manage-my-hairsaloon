using manage_my_hairsaloon.DTOs;
using manage_my_hairsaloon.Models;
using manage_my_hairsaloon.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace manage_my_hairsaloon.Controllers
{
    [Route("api/services")]
    [ApiController]
    public class ServicesApiController : ControllerBase
    {
        private readonly IServiceRepository _repository;

        public ServicesApiController(IServiceRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ServiceDTO>> Get(
            [FromQuery] string? category,
            [FromQuery] string? name,
            [FromQuery] string? salonName,
            [FromQuery] decimal? maxPrice)
        {
            var services = _repository.Filter(category, name, salonName, maxPrice);
            return Ok(services.Select(ToDTO));
        }

        [HttpGet("{id}")]
        public ActionResult<ServiceDTO> Get(int id)
        {
            var service = _repository.GetById(id);

            if (service == null)
                return NotFound();

            return Ok(ToDTO(service));
        }

        [HttpPost]
        public ActionResult<ServiceDTO> Post([FromBody] ServiceRequest model)
        {
            var service = new Service
            {
                HairSalonId = model.HairSalonId,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                DurationMinutes = model.DurationMinutes,
                Category = model.Category
            };

            _repository.Add(service);

            return CreatedAtAction(nameof(Get), new { id = service.Id }, ToDTO(service));
        }

        [HttpPut("{id}")]
        public ActionResult<ServiceDTO> Put(int id, [FromBody] ServiceRequest model)
        {
            var service = _repository.GetById(id);

            if (service == null)
                return NotFound();

            service.HairSalonId = model.HairSalonId;
            service.Name = model.Name;
            service.Description = model.Description;
            service.Price = model.Price;
            service.DurationMinutes = model.DurationMinutes;
            service.Category = model.Category;

            _repository.Update(service);

            return Ok(ToDTO(service));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var service = _repository.GetById(id);

            if (service == null)
                return NotFound();

            _repository.Delete(id);

            return NoContent();
        }

        private static ServiceDTO ToDTO(Service service) => new()
        {
            Id = service.Id,
            Name = service.Name,
            Description = service.Description,
            Price = service.Price,
            DurationMinutes = service.DurationMinutes,
            Category = service.Category.ToString(),
            HairSalonName = service.HairSalon?.Name
        };
    }
}
