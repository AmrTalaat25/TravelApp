using Microsoft.AspNetCore.Mvc;
using TravelApp.Dto;

namespace TravelApp.Models.Services
{
    public interface IPaymentService
    {
        Task<PaymentDTO> GetPaymentById(int paymentId);
        Task<IEnumerable<PaymentDTO>> GetAllPayments();
        Task<ActionResult<PaymentDTO>> CreatePayment(CreatePaymentDTO paymentcreateDTO);
        ActionResult<string> UpdatePayment(int paymentId, UpdatePaymentDTO updatePaymentDTO);
        ActionResult<string> DeletePayment(int paymentId);
    }
}