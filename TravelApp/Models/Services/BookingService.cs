using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelApp.Dto;
using TravelApp.Models.Services.Interfaces;

namespace TravelApp.Models.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;

        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }


        #region GetAllBookings
        public async Task<ActionResult<IEnumerable<BookingDTO>>> GetAllBookings()
        {
            var bookings = await _context.Bookings
                .Select(b => new BookingDTO
                {
                    BookingID = b.BookingID,
                    AdID = b.AdID,
                    UserID = b.UserID,
                    NumberOfSeats = b.NumberOfSeats,
                    BookingDate = b.BookingDate,
                    Status = b.Status,
                    TotalAmount = b.TotalAmount,
                    PaymentMethod = b.PaymentMethod,
                    PaymentDetails = b.PaymentDetails
                })
                .ToListAsync();

            return bookings;
        }
        #endregion

        #region GetByIdBooking
        public async Task<ActionResult<BookingDTO>> GetByIdBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                throw new Exception("This Booking Not Found");
            }

            var bookingDTO = new BookingDTO
            {
                BookingID = booking.BookingID,
                AdID = booking.AdID,
                UserID = booking.UserID,
                NumberOfSeats = booking.NumberOfSeats,
                BookingDate = booking.BookingDate,
                Status = booking.Status,
                TotalAmount = booking.TotalAmount,
                PaymentMethod = booking.PaymentMethod,
                PaymentDetails= booking.PaymentDetails
            };

            return bookingDTO;
        }
        #endregion

        #region CreateBooking
        public async Task<ActionResult<BookingDTO>> CreateBooking(CreateBookingDTO createBookingDTO)
        {
            var advertisement = await _context.Advertisements.FindAsync(createBookingDTO.AdID);

            if (advertisement == null)
            {
                // Handle case where advertisement is not found
                return NotFound("Advertisement not found");
            }
            var numberOfSeats = createBookingDTO.NumberOfSeats <= 0 ? 1 : createBookingDTO.NumberOfSeats;

            var totalAmount = advertisement.price * numberOfSeats;
            var booking = new Booking
            {
                AdID = createBookingDTO.AdID,
                UserID = createBookingDTO.UserID,
                NumberOfSeats = createBookingDTO.NumberOfSeats <= 0 ? 1 : createBookingDTO.NumberOfSeats,
                BookingDate = createBookingDTO.BookingDate,
                Status = createBookingDTO.Status,
                PaymentMethod = createBookingDTO.PaymentMethod,
                PaymentDetails = createBookingDTO.PaymentDetails,
                TotalAmount = totalAmount
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var bookingDTO = new BookingDTO
            {
                BookingID = booking.BookingID,
                AdID = booking.AdID,
                UserID = booking.UserID,
                NumberOfSeats = booking.NumberOfSeats,
                BookingDate = booking.BookingDate,
                Status = booking.Status,
                TotalAmount = totalAmount ,
                PaymentMethod = booking.PaymentMethod,
                PaymentDetails= booking.PaymentDetails
                
            };

            return bookingDTO;
        }
        private ActionResult<BookingDTO> NotFound(string v)
        {
            throw new NotImplementedException("Advertisement not found");
        }
        #endregion


        #region UpdateBooking
        public ActionResult<string> UpdateBooking(int id, UpdateBookingDTO updateBookingDTO)
        {

            var existingBooking = _context.Set<Booking>().Find(id);
            if (existingBooking != null)
            {
                existingBooking.NumberOfSeats = updateBookingDTO.NumberOfSeats;
                existingBooking.BookingDate = updateBookingDTO.BookingDate;
                existingBooking.Status = updateBookingDTO.Status;
                existingBooking.PaymentMethod = updateBookingDTO.PaymentMethod;
                existingBooking.PaymentDetails = updateBookingDTO.PaymentDetails;
                _context.SaveChangesAsync();
                return "Booking is Updated";

            }
            return "Not Found";
        }
        #endregion

        #region DeleteBooking
        public async Task<ActionResult<string>> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return "This booking Not Found";
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return "booking is Deleted";

        }
        #endregion
    }
}