using Microsoft.AspNetCore.Mvc;
using TravelApp.Dto;

namespace TravelApp.Models.Services.Interfaces
{
    public interface IBookingService
    {
        Task<ActionResult<BookingDTO>> CreateBooking(CreateBookingDTO createBookingDTO);
        Task<ActionResult<string>> DeleteBooking(int id);
        Task<ActionResult<IEnumerable<BookingDTO>>> GetAllBookings();
        Task<ActionResult<BookingDTO>> GetByIdBooking(int id);
        ActionResult<string> UpdateBooking(int id, UpdateBookingDTO updateBookingDTO);
    }
}