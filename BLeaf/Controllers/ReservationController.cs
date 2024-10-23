using Microsoft.AspNetCore.Mvc;
using BLeaf.Models;
using BLeaf.Models.IRepository;

namespace BLeaf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : Controller
    {
        private readonly IReservationRepository _reservationRepository;

        public ReservationController(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                reservation.CreatedAt = DateTime.Now;
                reservation.UpdatedAt = DateTime.Now;
                await _reservationRepository.AddReservationAsync(reservation);
                return Json(new { success = true, message = "Reservation created successfully!" });
            }
            return Json(new { success = false, message = "Failed to create reservation. Please check your input and try again." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReservations()
        {
            var reservations = await _reservationRepository.GetAllReservationsAsync();
            return Ok(reservations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservationById(int id)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            return Ok(reservation);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(int id, [FromBody] Reservation reservation)
        {
            if (reservation == null || reservation.ReservationId != id)
            {
                return BadRequest(new { success = false, message = "Invalid reservation data." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid model state." });
            }

            try
            {
                reservation.UpdatedAt = DateTime.Now;
                var updatedReservation = await _reservationRepository.UpdateReservationAsync(reservation);
                return Ok(new { success = true, message = "Reservation updated successfully!", data = updatedReservation });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Reservation not found." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            try
            {
                var deletedReservation = await _reservationRepository.DeleteReservationAsync(id);
                return Ok(new { success = true, message = "Reservation deleted successfully!", data = deletedReservation });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}