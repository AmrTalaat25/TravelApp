using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TravelApp.Dto;
using TravelApp.Models.Services.Interfaces;

namespace TravelApp.Models.Services
{
    public class OfferService : IOfferService
    {


        private readonly ApplicationDbContext _context;

        public OfferService(ApplicationDbContext context)
        {
            _context = context;
        }
        #region All Offers
        public async Task<IEnumerable<OfferDTO>> GetAllOffers()
        {
            var offer = await _context.Offers
        .Select(o => new OfferDTO
        {
            AdID = o.AdID,
            OfferID = o.OfferID,
            OfferName = o.OfferName,
            Details = o.details,
            DiscountAmount = o.DiscountAmount,
            PostDate = o.post_date,
            ExpiryDate = o.expiry_date

        })
        .ToListAsync();
            return offer;
        }
        #endregion



        #region  Offer ById
        public async Task<OfferDTO> GetByIdOffer(int id)
        {
            var offer = await _context.Offers.FindAsync(id);

            if (offer == null)
            {
                return null!;
            }

            var offerDTO = new OfferDTO
            {
                AdID = offer.AdID,
                OfferID = offer.OfferID,
                OfferName = offer.OfferName,
                Details = offer.details,
                DiscountAmount = offer.DiscountAmount,
                PostDate = offer.post_date,
                ExpiryDate = offer.expiry_date
            };

            return offerDTO;
        }

        #endregion


        #region Create Offer
        public async Task<OfferDTO> CreateNewOffer(CreateOfferDTO createOfferDTO)
        {
            var offer = new Offer
            {
                AdID = createOfferDTO.AdID,
                OfferName = createOfferDTO.OfferName,
                details = createOfferDTO.Details,
                DiscountAmount = createOfferDTO.DiscountAmount,
                post_date = createOfferDTO.PostDate,
                expiry_date = createOfferDTO.ExpiryDate
            };

            _context.Offers.Add(offer);
            await _context.SaveChangesAsync();

            var offerDTO = new OfferDTO
            {
                AdID = offer.AdID,
                OfferID = offer.OfferID,
                OfferName = offer.OfferName,
                Details = offer.details,
                DiscountAmount = offer.DiscountAmount,
                PostDate = offer.post_date,
                ExpiryDate = offer.expiry_date

            };

            return offerDTO;
        }
        #endregion


        #region Update Offer
        public ActionResult<string> UpdateOffer(int id, UpdateOfferDTO updateOfferDTO)
        {


            var existingOffer = _context.Set<Offer>().Find(id);
            if (existingOffer != null)
            {
                existingOffer.OfferName = updateOfferDTO.OfferName;
                existingOffer.details = updateOfferDTO.Details;
                existingOffer.DiscountAmount = updateOfferDTO.DiscountAmount;
                existingOffer.post_date = updateOfferDTO.PostDate;
                existingOffer.expiry_date = updateOfferDTO.ExpiryDate;
                _context.SaveChanges();
                return "Offer is Updated";
            }
            return "not found";

        }
        #endregion

        #region Delete Offer
        public ActionResult<string> DeleteOffer(int id)
        {

            var existingOffer = _context.Set<Offer>().Find(id);
            if (existingOffer != null)
            {
                _context.Set<Offer>().Remove(existingOffer);
                _context.SaveChanges();
                return "Offer is deleted";
            }
            else
                return "not found Offer";
        }
        #endregion


    }
}
