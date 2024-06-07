using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelApp.Dto;

namespace TravelApp.Models.Services
{
    public class PaymentService : IPaymentService
    {

        private readonly ApplicationDbContext _context;

        public PaymentService(ApplicationDbContext context)
        {
            _context = context;
        }
        #region CreatePayment
        public async Task<ActionResult<PaymentDTO>> CreatePayment(CreatePaymentDTO paymentcreateDTO)
        {
            var payment = new Payment
            {
                BookingID = paymentcreateDTO.BookingId,
                PaymentMethod = paymentcreateDTO.PaymentMethod,
                PaymentDetails = paymentcreateDTO.PaymentDetails
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            var paymentDTO = new PaymentDTO
            {
                BookingId = payment.BookingID,
                PaymentMethod = payment.PaymentMethod,
                PaymentDetails = payment.PaymentDetails

            };

            return paymentDTO;
        }
        #endregion

        #region GetAllPayments
        public async Task<IEnumerable<PaymentDTO>> GetAllPayments()
        {
            var payments = await _context.Payments
                .Select(payment => new PaymentDTO
                {
                    PaymentId = payment.PaymentID,
                    BookingId = payment.BookingID,
                    PaymentMethod = payment.PaymentMethod,
                    PaymentDetails = payment.PaymentDetails
                })
                .ToListAsync();

            return payments;
        }
        #endregion

        #region GetPaymentById
        public async Task<PaymentDTO> GetPaymentById(int paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);

            if (payment == null)
            {
                return null!; // or throw exception, handle as needed
            }

            var paymentDTO = new PaymentDTO
            {
                PaymentId = payment.PaymentID,
                BookingId = payment.BookingID,
                PaymentMethod = payment.PaymentMethod,
                PaymentDetails = payment.PaymentDetails
            };

            return paymentDTO;
        }
        #endregion

        #region Update Payment
        public ActionResult<string> UpdatePayment(int paymentId, UpdatePaymentDTO updatePaymentDTO)
        {
            var existingPayment = _context.Set<Payment>().Find(paymentId);
            if (existingPayment != null)
            {
                existingPayment.PaymentMethod = updatePaymentDTO.PaymentMethod;
                existingPayment.PaymentDetails = updatePaymentDTO.PaymentDetails;
                _context.SaveChanges();
                return "Payment is Updated";
            }
            return "not found";
        }
        #endregion

        #region Delete Payment
        public ActionResult<string> DeletePayment(int paymentId)
        {
            var existingPayment = _context.Set<Payment>().Find(paymentId);
            if (existingPayment != null)
            {
                _context.Set<Payment>().Remove(existingPayment);
                _context.SaveChanges();
                return "Payment is deleted";
            }
            else
                return "not found Offer";
        }
        #endregion
    }
}
