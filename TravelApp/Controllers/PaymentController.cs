using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelApp.Dto; 
using TravelApp.Models.Services; 

namespace TravelApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService Service)
        {
            _service = Service;
        }
        // GET: api/Payment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDTO>>> GetAllPayments()
        {
            IEnumerable<PaymentDTO> payment = await _service.GetAllPayments();
            if (payment == null || !payment.Any())
            {
                return NotFound("No Payments were found in the database");
            }
            return Ok(payment);

        }

        // GET: api/Payment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDTO>> GetByIdPayment(int id)
        {
            var paymentDTO = await _service.GetPaymentById(id);

            if (paymentDTO == null)
            {
                return NotFound($"Offer with ID {id} Does not exist");
            }

            return Ok(paymentDTO);
        }

        // PUT: api/Payment/5
        [HttpPut("{id}")]
        public ActionResult<string> UpdatePayment(int paymentId, UpdatePaymentDTO updatePaymentDTO)
        {
            return _service.UpdatePayment(paymentId, updatePaymentDTO);
        }

        // POST: api/Payment
        [HttpPost]
        public async Task<ActionResult<PaymentDTO>> CreatePayment(CreatePaymentDTO paymentcreateDTO)
        {
            

            return await _service.CreatePayment(paymentcreateDTO);
        }

        // DELETE: api/Payment/5
        [HttpDelete("{id}")]
        public ActionResult<string> DeletePayment(int paymentId)
        {return _service.DeletePayment(paymentId);
        }
    }
}
