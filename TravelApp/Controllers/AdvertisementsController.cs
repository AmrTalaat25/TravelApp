using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TravelApp.Dto;

using TravelApp.Models;
using TravelApp.Models.Services.Interfaces;

namespace TravelApp.Controllers
{
    [Route("api/advertisements")]
    [ApiController]
    public class AdvertisementsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAdvertisementService _service;

        public AdvertisementsController(ApplicationDbContext context, IAdvertisementService service)
        {
            _context = context;
            _service = service;
        }

        // GET: api/advertisements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdvertisementDTO>>> GetAdvertisements()
        {
            IEnumerable<AdvertisementDTO> advertisements = await _service.GetAllAdvertisements();
            if (advertisements == null || !advertisements.Any())
            {
                return NotFound("No Advertisements were found in the database");
            }
            return Ok(advertisements);
        }

        // GET: api/advertisements/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AdvertisementDTO>> GetAdvertisement(int id)
        {
            var advertisementDTO = await _service.GetByIdAdvertisement(id);
            if (advertisementDTO == null)
            {
                return NotFound($"Advertisement with ID {id} Does not exist");
            }
            return Ok(advertisementDTO);
        }

        [HttpPost]
        [Authorize(Roles = "CompanyOwner")]
        public async Task<ActionResult<ResponseModel<AdvertisementDTO>>> CreateNewAdvertisement([FromForm] CreateAdvertisementDTO createAdvertisementDTO, [FromForm] List<IFormFile> images)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "uid")!.Value;

                var CompanyExists = await _context.Companies.AnyAsync(c => c.CompanyID == createAdvertisementDTO.CompanyID);

                if (!CompanyExists)
                {
                    return NotFound($"No company with ID {createAdvertisementDTO.CompanyID} Found");
                }

                var userOwnedCompanies = await _context.Companies.Where(c => c.UserID == userId).ToListAsync();

                if (!userOwnedCompanies.Any(c => c.CompanyID == createAdvertisementDTO.CompanyID))
                {
                    return StatusCode(403, new ResponseModel<AdvertisementDTO>
                    {
                        Message = "You are Forbidden from creating an advertisement for this company",
                        Success = false
                    });
                }

                var result = await _service.CreateNewAdvertisement(createAdvertisementDTO, images);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            else return BadRequest();
        }


        // PUT: api/advertisements/{id}
        [HttpPut("{id}")]
        public ActionResult<string> UpdateAdvertisement(int id, UpdateAdvertisementDTO updateAdvertisementDTO)
        {
            return _service.UpdateAdvertisement(id, updateAdvertisementDTO);
        }

        // DELETE: api/advertisements/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseModel<List<string>>>> DeleteAdvertisement(int id)
        {
            var result = await _service.DeleteAdvertisement(id);
            if (!result.Success)
            {
                BadRequest(result);
            }
            return Ok(result);
        }


        [HttpGet("ByTitle/{AdvertisementTitle}")]
        public async Task<ActionResult<IEnumerable<AdvertisementDTO>>> GetAdvertisementByTitle(string AdvertisementTitle)
        {
            IEnumerable<AdvertisementDTO> advertisements = await _service.GetAdvertisementByTitle(AdvertisementTitle);
            if (advertisements == null)
            {
                return NotFound($"Could not find any advertisements with title '{AdvertisementTitle}'");
            }
            return Ok(advertisements);
        }

        [HttpGet("{id}/reviews")]
        public async Task<ActionResult<AdvertisementReviewsDTO>> GetReviewsByAdvertisementID(int id)
        {
            var advertisementReviewsDTO = await _service.GetReviewsByAdvertisementID(id);

            if (advertisementReviewsDTO == null)
            {
                return NotFound($"No reviews found for advertisement with ID {id}");
            }

            return Ok(advertisementReviewsDTO);
        }
    }
}