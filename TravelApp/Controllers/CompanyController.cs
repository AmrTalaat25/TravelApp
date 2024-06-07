using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using TravelApp.Dto;
using TravelApp.Models;
using TravelApp.Models.Services.Interfaces;

namespace TravelApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICompanyService _service;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CompanyController(ApplicationDbContext context,ICompanyService service, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _service = service;
            _webHostEnvironment = webHostEnvironment;
        }

        // NEW -----------

        [HttpPost("createCompany")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCompany([FromForm] AdminCreateCompanyDTO companyDTO)
        {
            var result = await _service.AdminCreateCompany(companyDTO);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var result = await _service.GetAllCompanies();
            if (result.Success)
            {
                return Ok(result);
            }
            else { return BadRequest(result); }
        }

        //  api/Company/int id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompany(int id)
        {
            var result = await _service.GetByIdCompany(id);
            if (result.Success)
            {
                return Ok(result);
            }
            else { return BadRequest(result); }
        }

        // DELETE: api/Company/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var result = await _service.DeleteCompany(id);
            if (result.Success)
            {
                return Ok(result);
            }
            else { return BadRequest(result); }
        }

        // api/Company/int id
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(int id, UpdateCompanyDTO companyDTO)
        {
            var result = await _service.UpdateCompany(id, companyDTO);
            if (result.Success)
            {
                return Ok(result);
            }
            else { return BadRequest(result); }
        }

        [HttpGet("GetUserBookedWithCompany/{companyId}")]
        public async Task<IActionResult> GetUsersForCompanyBooking(int companyId)
        {
            var result = await _service.GetUsersForCompanyBooking(companyId);
            if (result.Success)
            {
                return Ok(result);
            }
            else { return BadRequest(result); }
        }

        // api/Company

        [HttpGet("ByUser/{userId}")]
        public async Task<IActionResult> GetCompaniesByUserId(string userId)
        {
            var companies = await _service.GetCompaniesByUserId(userId);
            if (companies == null)
            {
                return NotFound();
            }
            return Ok(companies);
        }

        [HttpGet("ByName/{companyname}")]
        public async Task<IActionResult> GetCompaniesByCompanyName(string companyname)
        {
            var companies = await _service.GetCompaniesByCompanyName(companyname);
            if (companies == null)
            {
                return NotFound();
            }
            return Ok(companies);
        }

        [HttpGet("{id}/AllAdvertisements")]
        public async Task<IActionResult> GetAllAdvertisementByIDCompany(int id)
        {
            var advertisements = await _service.GetAllAdvertisementByIDCompany(id);
            if (advertisements == null)
            {
                return NotFound();
            }
            return Ok(advertisements);
        }


        // Company logo controller
        [HttpPost("SetLogo/{companyId}")]
        public async Task<IActionResult> SetCompanyLogo(int companyId, IFormFile logo)
        {
            var result = await _service.SetCompanyLogo(companyId, logo);
            if(result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        // Get Company Logo Base64
        [HttpGet("GetLogo/{companyId}")]
        public async Task<IActionResult> GetCompanyLogo(int companyId)
        {
            var company = await _context.Companies.FindAsync(companyId);
            if (company == null)
            {
                return BadRequest("Failed to find company");
            }
            var logoPath = Path.Combine(_webHostEnvironment.WebRootPath, company.LogoPath);
            byte[] logo = await System.IO.File.ReadAllBytesAsync(logoPath);
            return Ok(logo);
        }
    }
}
