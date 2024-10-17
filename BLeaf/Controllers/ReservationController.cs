using Microsoft.AspNetCore.Mvc;
using BLeaf.Models;
using BLeaf.Models.IRepository;
using System.Threading.Tasks;

namespace BLeaf.Controllers
{
    public class ReservationController : Controller
    {
        private readonly IReservationRepository _reservationRepository;

        public ReservationController(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Reservation reservation)
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

        public async Task<IActionResult> ManageReservations()
        {
            var reservations = await _reservationRepository.GetAllReservationsAsync();
            return View(reservations);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateReservationStatus(int reservationId, string status)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(reservationId);
            if (reservation != null)
            {
                reservation.ReservationStatus = status;
                reservation.UpdatedAt = DateTime.Now;
                await _reservationRepository.UpdateReservationAsync(reservation);
                return Json(new { success = true, message = "Reservation status updated successfully!" });
            }
            return Json(new { success = false, message = "Failed to update reservation status. Reservation not found." });
        }
    }
}