using Microsoft.AspNetCore.Mvc;
using TravelApp.Dto;

namespace TravelApp.Models.Services
{
    public interface IOfferService
    {
        Task<OfferDTO> CreateNewOffer(CreateOfferDTO createOfferDTO);
        ActionResult<string> DeleteOffer(int id);
        Task<IEnumerable<OfferDTO>> GetAllOffers();
        Task<OfferDTO> GetByIdOffer(int id);
        ActionResult<string> UpdateOffer(int id, UpdateOfferDTO updateOfferDTO);
    }
}