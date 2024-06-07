using Microsoft.AspNetCore.Mvc;
using TravelApp.Dto;

namespace TravelApp.Models.Services.Interfaces
{
    public interface IAdvertisementService
    {
        Task<ResponseModel<AdvertisementDTO>> CreateNewAdvertisement(CreateAdvertisementDTO ad, List<IFormFile> images);
        Task<ResponseModel<List<string>>> DeleteAdvertisement(int id);
        Task<IEnumerable<AdvertisementDTO>> GetAllAdvertisements();
        Task<AdvertisementDTO> GetByIdAdvertisement(int id);
        ActionResult<string> UpdateAdvertisement(int id, UpdateAdvertisementDTO updateAdvertisementDTO);
        Task<IEnumerable<AdvertisementDTO>> GetAdvertisementByTitle(string AdvertisementTitle);
        Task<AdvertisementReviewsDTO> GetReviewsByAdvertisementID(int id);
    }
}