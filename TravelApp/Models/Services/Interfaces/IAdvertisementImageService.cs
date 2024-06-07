using TravelApp.Dto;

namespace TravelApp.Models.Services.Interfaces
{
    public interface IAdvertisementImageService
    {
        Task<bool> AddImage(AdvertisementImageDto model);
        Task<List<string>> GetImagesByAdID(int adID);
        Task<string> GetImageBase64ById(int imageId);
        Task<string> DeleteImageById(int imageId);
        Task<bool> UpdateImage(UpdateImageDto updateImageDto);
        Task<ResponseModel<List<string>>> DeleteAdvertisementImages(int AdId);
    }
}