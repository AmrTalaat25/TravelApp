using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using TravelApp.Dto;
using TravelApp.Models.Services.Interfaces;

namespace TravelApp.Models.Services
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAdvertisementImageService _imageService;

        public AdvertisementService(ApplicationDbContext context, IAdvertisementImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        #region AdvertisementByTitle
        public async Task<IEnumerable<AdvertisementDTO>> GetAdvertisementByTitle(string AdvertisementTitle)
        {
            var advertisements = await _context.Advertisements
                .Where(a => a.Title.Contains(AdvertisementTitle))
                .ToListAsync();

            if (advertisements == null || advertisements.Count == 0)
            {
                return null!;
            }

            var advertisementDTOs = new List<AdvertisementDTO>();

            foreach (var advertisement in advertisements)
            {
                var advertisementDTO = new AdvertisementDTO
                {
                    AdID = advertisement.AdID,
                    Title = advertisement.Title,
                    price = advertisement.price,
                    CompanyName = advertisement.CompanyName,
                    Description = advertisement.Description,
                    ValidFrom = advertisement.ValidFrom,
                    ValidTo = advertisement.ValidTo,
                    TravelTo = advertisement.TravelTo,
                    TravelFrom = advertisement.TravelFrom
                };

                advertisementDTO.Base64Images = await _imageService.GetImagesByAdID(advertisement.AdID);

                advertisementDTOs.Add(advertisementDTO);
            }

            return advertisementDTOs;
        }


        #endregion

        #region All Advertisements
        public async Task<IEnumerable<AdvertisementDTO>> GetAllAdvertisements()
        {
            var advertisements = await _context.Advertisements.ToListAsync();

            if (advertisements == null || advertisements.Count == 0)
            {
                return null!;
            }

            var advertisementDTOs = new List<AdvertisementDTO>();

            foreach (var advertisement in advertisements)
            {
                var advertisementDTO = new AdvertisementDTO
                {
                    AdID = advertisement.AdID,
                    Title = advertisement.Title,
                    price = advertisement.price,
                    CompanyName = advertisement.CompanyName,
                    Description = advertisement.Description,
                    ValidFrom = advertisement.ValidFrom,
                    ValidTo = advertisement.ValidTo,
                    TravelTo = advertisement.TravelTo,
                    TravelFrom = advertisement.TravelFrom
                };

                advertisementDTO.Base64Images = await _imageService.GetImagesByAdID(advertisement.AdID);

                advertisementDTOs.Add(advertisementDTO);
            }

            return advertisementDTOs;
        }
        #endregion

        #region  Advertisement ById
        public async Task<AdvertisementDTO> GetByIdAdvertisement(int id)
        {
            var advertisement = await _context.Advertisements.FindAsync(id);

            if (advertisement == null)
            {
                return null!;
            }

            var advertisementDTO = new AdvertisementDTO
            {
                AdID = advertisement!.AdID,
                CompanyName = advertisement.CompanyName,
                Title = advertisement.Title,
                price = advertisement.price,
                Description = advertisement.Description,
                ValidFrom = advertisement.ValidFrom,
                ValidTo = advertisement.ValidTo,
            };
            advertisementDTO.Base64Images = await _imageService.GetImagesByAdID(advertisement.AdID);

            return advertisementDTO;
        }

        #endregion 

     #region Create Advertisement
     public async Task<ResponseModel<AdvertisementDTO>> CreateNewAdvertisement(CreateAdvertisementDTO ad, List<IFormFile> images)
     {
         var advertisement = new Advertisement
         {
             CompanyID = ad.CompanyID,
             CompanyName = ad.CompanyName,
             Title = ad.Title,
             price = ad.price,
             Description = ad.Description,
             ValidFrom = ad.ValidFrom,
             ValidTo = ad.ValidTo,
             TravelTo = ad.TravelTo,
             TravelFrom = ad.TravelFrom
         };
         _context.Advertisements.Add(advertisement);
         await _context.SaveChangesAsync();
         var adid = advertisement.AdID;

         List<string> imageNames = new();
         foreach (var image in images)
         {
             try
             {
                 await _imageService.AddImage(new AdvertisementImageDto
                 {
                     AdID = adid,
                     Image = image
                 });
                 imageNames.Add($"Uploaded image {image.FileName}");
             }
             catch (Exception ex)
             {
                 imageNames.Add($"{ex.Message}");
             }
         }
         var ImageUrls = await _imageService.GetImagesByAdID(adid);
         var response = new ResponseModel<AdvertisementDTO>
         {
             Success = true,
             Message = $"Added images {imageNames} Successfully",
             Data = new AdvertisementDTO
             {
                 AdID = advertisement.AdID,
                 CompanyName = advertisement.CompanyName,
                 Title = advertisement.Title,
                 price = advertisement.price,
                 Description = advertisement.Description,
                 ValidFrom = advertisement.ValidFrom,
                 ValidTo = advertisement.ValidTo,
                 Base64Images = ImageUrls
             }
         };
         return response;
     }
        #endregion

        #region Update Advertisement
        public ActionResult<string> UpdateAdvertisement(int id, UpdateAdvertisementDTO updateAdvertisementDTO)
        {
            var existingAdvertisement = _context.Set<Advertisement>().Find(id);
            if (existingAdvertisement != null)
            {
                existingAdvertisement.Title = updateAdvertisementDTO.Title;
                existingAdvertisement.price = updateAdvertisementDTO.price;
                existingAdvertisement.Description = updateAdvertisementDTO.Description;
                existingAdvertisement.ValidFrom = updateAdvertisementDTO.ValidFrom;
                existingAdvertisement.ValidTo = updateAdvertisementDTO.ValidTo;
                existingAdvertisement.TravelTo = updateAdvertisementDTO.TravelTo;
                existingAdvertisement.TravelFrom = updateAdvertisementDTO.TravelFrom;
                _context.SaveChanges();
                return "Advertisement is Updated";
            }
            return "not found";

        }
        #endregion

        #region Delete Advertisement
        public async Task<ResponseModel<List<string>>> DeleteAdvertisement(int id)
        {

            var existingAdvertisement = _context.Set<Advertisement>().Find(id);
            if (existingAdvertisement == null)
            {
                return new ResponseModel<List<string>>
                {
                    Message = "Could not find advertisement",
                    Success = false
                };
            }
            try
            {
                var result = await _imageService.DeleteAdvertisementImages(id);
                _context.Set<Advertisement>().Remove(existingAdvertisement);
                _context.SaveChanges();
                return new ResponseModel<List<string>>
                {
                    Message = "Image Deletion Complete",
                    Data = result.Data
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<List<string>>
                {
                    Message = "Image Deletion Failed",
                    Success = false,
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        #endregion

        #region Get Reviews By Advertisement ID
        public async Task<AdvertisementReviewsDTO> GetReviewsByAdvertisementID(int id)
        {
            var advertisement = await _context.Advertisements
                .Include(a => a.Reviews)
                .FirstOrDefaultAsync(a => a.AdID == id);

            if (advertisement == null)
            {
                throw new Exception("Advertisement doesn't exist");
            }

            if (advertisement.Reviews == null || !advertisement.Reviews.Any())
            {
                return null!;
            }

            var advertisementReviewsDTO = new AdvertisementReviewsDTO
            {
                Reviews = advertisement.Reviews.Select(r => new ReviewDTO
                {
                    ReviewID = r.ReviewID,
                    AdID = r.AdID,
                    UserName = r.Username,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    DatePosted = r.DatePosted
                }).ToList()
            };

            return advertisementReviewsDTO;
        }
        #endregion
    }
}
