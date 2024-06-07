using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using TravelApp.Dto;
using TravelApp.Models;
using TravelApp.Models.Services;
using TravelApp.Models.Services.Interfaces;

namespace TravelApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdvertisementImageController : ControllerBase
    {
        private readonly IAdvertisementImageService _service;

        public AdvertisementImageController(IAdvertisementImageService service)
        {
            _service = service;
        }

        [HttpPost("AddImage")]
        public async Task<IActionResult> AddImage([FromForm] AdvertisementImageDto model)
        {
            try
            {
                var result = await _service.AddImage(model);
                return result ? Ok("Image uploaded successfully!") : BadRequest("Failed to upload image.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to upload image: {ex.Message}");
            }
        }

        [HttpGet("GetImagesByAdID/{adID}")]
        public async Task<IActionResult> GetImagesByAdID(int adID)
        {
            try
            {
                var imageBase64Urls = await _service.GetImagesByAdID(adID);
                if (imageBase64Urls == null)
                {
                    return NotFound($"Advertisement with ID {adID} not found or images could not be retrieved.");
                }

                return Ok(imageBase64Urls);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to retrieve images: {ex.Message}");
            }
        }

        [HttpGet("GetImageBase64ById/{imageId}")]
        public async Task<IActionResult> GetImageBase64ById(int imageId)
        {
            try
            {
                var base64Url = await _service.GetImageBase64ById(imageId);
                if (base64Url == null)
                {
                    return NotFound($"Image with ID {imageId} not found or could not be retrieved.");
                }

                return Ok(base64Url);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to retrieve image: {ex.Message}");
            }
        }

        [HttpDelete("DeleteImageById/{imageId}")]
        public async Task<IActionResult> DeleteImageById(int imageId)
        {
            var result = await _service.DeleteImageById(imageId);
            return Ok(result);
        }

        [HttpDelete("DeleteAdImages/{AdId}")]
        public async Task<ActionResult<ResponseModel<List<string>>>> DeleteAdvertisementImages(int AdId)
        {
            var result = await _service.DeleteAdvertisementImages(AdId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("UpdateImage")]
        public async Task<IActionResult> UpdateImage([FromForm] UpdateImageDto updateImageDto)
        {
            try
            {
                var result = await _service.UpdateImage(updateImageDto);
                if (!result)
                {
                    return NotFound($"Image with ID {updateImageDto.ImageId} not found or could not be updated.");
                }

                return Ok("Image updated successfully!");
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Failed to update image: {ex.Message}");
            }
            catch (FileNotFoundException ex)
            {
                return BadRequest($"Failed to update image: {ex.Message}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update image: {ex.Message}");
            }
        }
    }
}
