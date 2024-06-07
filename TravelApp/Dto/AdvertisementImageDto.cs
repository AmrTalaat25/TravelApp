using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TravelApp.Dto
{
    public class AdvertisementImageDto
    {
        public int AdID { get; set; }
        public IFormFile Image { get; set; }
    }
    public class UpdateImageDto
    {
        public int ImageId { get; set; }
        public IFormFile NewImage { get; set; }
    }
}
