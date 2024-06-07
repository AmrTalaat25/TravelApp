using Microsoft.AspNetCore.Mvc;
using TravelApp.Models.Services.Interfaces;
using TravelApp.Models;
using TravelApp.Models.Services;
using TravelApp.Dto;

namespace TravelApp.Controllers
{
    [Route("api/offers")]
    [ApiController]
    public class OffersController: ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IOfferService _service;

        public OffersController(ApplicationDbContext context, IOfferService service)
        {
            _context = context;
            _service = service;
        }
        // GET: api/offer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OfferDTO>>> GetAllOffers()
        {
            IEnumerable<OfferDTO> offer = await _service.GetAllOffers();
            if (offer == null || !offer.Any())
            {
                return NotFound("No Offers were found in the database");
            }
            return Ok(offer);
        }

        // GET: api/offer/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OfferDTO>> GetByIdOffers(int id)
        {
            var offerDTO = await _service.GetByIdOffer(id);
            if (offerDTO == null)
            {
                return NotFound($"Offer with ID {id} Does not exist");
            }
            return Ok(offerDTO);
        }
        // POST: api/bookings
        [HttpPost]
        public async Task<OfferDTO> CreateNewOffer(CreateOfferDTO createOfferDTO)
        {
            return await _service.CreateNewOffer(createOfferDTO);
        }


        // PUT: api/offer/{id}
        [HttpPut("{id}")]
        public ActionResult<string> UpdateOffer(int id, UpdateOfferDTO updateOfferDTO)
        {
            return _service.UpdateOffer(id, updateOfferDTO);
        }

        // DELETE: api/offer/{id}
        [HttpDelete("{id}")]
        public ActionResult<string> DeleteOffer(int id)
        {
            return _service.DeleteOffer(id);
        }
    }
}
