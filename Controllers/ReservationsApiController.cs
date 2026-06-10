using manage_my_hairsaloon.DTOs;
using manage_my_hairsaloon.Models;
using manage_my_hairsaloon.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace manage_my_hairsaloon.Controllers
{
    [Route("api/reservations")]
    [ApiController]
    public class ReservationsApiController : ControllerBase
    {
        private readonly IReservationRepository _repository;

        public ReservationsApiController(IReservationRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ReservationDTO>> Get(
            [FromQuery] string? status,
            [FromQuery] string? customerName,
            [FromQuery] string? serviceName,
            [FromQuery] DateTime? dateFrom,
            [FromQuery] DateTime? dateTo)
        {
            var reservations = _repository.Filter(status, customerName, serviceName, dateFrom, dateTo);
            return Ok(reservations.Select(ToDTO));
        }

        [HttpGet("{id}")]
        public ActionResult<ReservationDTO> Get(int id)
        {
            var reservation = _repository.GetById(id);

            if (reservation == null)
                return NotFound();

            return Ok(ToDTO(reservation));
        }

        [HttpPost]
        public ActionResult<ReservationDTO> Post([FromBody] ReservationRequest model)
        {
            var reservation = new Reservation
            {
                CustomerId = model.CustomerId,
                StaffId = model.StaffId,
                ServiceId = model.ServiceId,
                ReservationDateTime = model.ReservationDateTime,
                Notes = model.Notes,
                Status = ReservationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _repository.Add(reservation);

            return CreatedAtAction(nameof(Get), new { id = reservation.Id }, ToDTO(reservation));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var reservation = _repository.GetById(id);

            if (reservation == null)
                return NotFound();

            _repository.Delete(id);

            return NoContent();
        }

        private static ReservationDTO ToDTO(Reservation reservation) => new()
        {
            Id = reservation.Id,
            ReservationDateTime = reservation.ReservationDateTime,
            Status = reservation.Status.ToString(),
            Notes = reservation.Notes,
            Customer = reservation.Customer == null ? null : new UserDTO
            {
                Id = reservation.Customer.Id,
                Email = reservation.Customer.Email,
                FirstName = reservation.Customer.FirstName,
                LastName = reservation.Customer.LastName,
                PhoneNumber = reservation.Customer.PhoneNumber,
                Role = reservation.Customer.Role.ToString()
            },
            Staff = reservation.Staff == null ? null : new StaffDTO
            {
                Id = reservation.Staff.Id,
                Specialization = reservation.Staff.Specialization,
                HourlyRate = reservation.Staff.HourlyRate,
                IsAvailable = reservation.Staff.IsAvailable
            },
            Service = reservation.Service == null ? null : new ServiceDTO
            {
                Id = reservation.Service.Id,
                Name = reservation.Service.Name,
                Description = reservation.Service.Description,
                Price = reservation.Service.Price,
                DurationMinutes = reservation.Service.DurationMinutes,
                Category = reservation.Service.Category.ToString()
            }
        };
    }
}
