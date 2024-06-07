using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TravelApp.Dto;
using TravelApp.Models;
using TravelApp.Models.Services.Interfaces;

namespace TravelApp.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IBookingService _service;

        public BookingsController(ApplicationDbContext context, IBookingService service )
        {
            _context = context;
            _service = service;
        }

        // GET: api/bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDTO>>> GetAllBookings()
        {
            return await _service.GetAllBookings();
        }

        // GET: api/bookings/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDTO>> GetByIdBooking(int id)
        {
            return await _service.GetByIdBooking(id);
        }

        // POST: api/bookings
        [HttpPost]
        public async Task<ActionResult<BookingDTO>> CreateBooking(CreateBookingDTO createBookingDTO)
        {
            return await _service.CreateBooking(createBookingDTO);
        }

        // PUT: api/bookings/{id}
        [HttpPut("{id}")]
        public ActionResult<string> UpdateBooking(int id, UpdateBookingDTO updateBookingDTO)
        {
            return _service.UpdateBooking(id, updateBookingDTO);
        }

        // DELETE: api/bookings/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteBooking(int id)
        {
            return await _service.DeleteBooking(id);
        }
    }
}
