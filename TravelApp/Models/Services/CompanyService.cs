using HandlebarsDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using System.Linq;
using System.Numerics;
using TravelApp.Dto;
using TravelApp.Models.Services.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TravelApp.Models.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CompanyService(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Create Company
        // service for permission request creation
        public async Task<bool> CreateCompany(CreateCompanyDTO company)
        {
            var permissionRequest = await _context.PermissionRequests.FindAsync(company.RequestID);
            var newCompany = new Company
            {
                UserID = company.UserID,
                CompanyName = company.CompanyName,
                CompanyAddress = company.CompanyAddress,
                ContactInformation = company.ContactInformation,
                PermissionRequest = permissionRequest
            };

            _context.Companies.Add(newCompany);
            await _context.SaveChangesAsync();
            return true;
        }

        // service for admin
        public async Task<ResponseModel<CompanyDTO>> AdminCreateCompany(AdminCreateCompanyDTO company)
        {
            var newCompany = new Company
            {
                UserID = company.UserID,
                CompanyName = company.CompanyName,
                CompanyAddress = company.CompanyAddress,
                ContactInformation = company.ContactInformation,
                PermissionRequest = null,
                Active = true
            };

            _context.Companies.Add(newCompany);
            await _context.SaveChangesAsync();

            return new ResponseModel<CompanyDTO>()
            {
                Message = "Company created successfully",
                Data = new CompanyDTO
                {
                    CompanyID = newCompany.CompanyID,
                    CompanyName = newCompany.CompanyName,
                    CompanyAddress = newCompany.CompanyAddress,
                    ContactInformation = newCompany.ContactInformation,
                    UserID = newCompany.UserID,
                }
            };
        }
        #endregion

        #region GetAllCompanies
        public async Task<ResponseModel<IEnumerable<CompanyDTO>>> GetAllCompanies()
        {
            try
            {
                var errors = new List<string>();
                var companies = await _context.Companies
                .Where(c => c.Active).ToListAsync();
                var companiesDTO = new List<CompanyDTO>();
                foreach (var company in companies)
                {
                    byte[] logoBytes;
                    string base64String = null!;
                    try
                    {
                        logoBytes = File.ReadAllBytes(company.LogoPath);
                        base64String = Convert.ToBase64String(logoBytes);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex.Message);
                    }
                    var companyDTO = new CompanyDTO
                    {
                        CompanyID = company.CompanyID,
                        CompanyName = company.CompanyName,
                        CompanyAddress = company.CompanyAddress,
                        ContactInformation = company.ContactInformation,
                        UserID = company.UserID,
                        LogoBase64 = base64String,
                    };
                    companiesDTO.Add(companyDTO);
                }

                return new ResponseModel<IEnumerable<CompanyDTO>>
                {
                    Message = "Retireved companies successfully",
                    Data = companiesDTO,
                    Errors = errors
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<IEnumerable<CompanyDTO>>
                {
                    Message = "Failed to retrieve companies",
                    Success = false,
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        #endregion

        #region GetByIdCompany
        public async Task<ResponseModel<CompanyDTO>> GetByIdCompany(int id)
        {
            try
            {
                var company = await _context.Companies
                .Where(c => c.CompanyID == id)
                .Where(c => c.Active)
                .SingleOrDefaultAsync();

                if (company == null)
                {
                    return new ResponseModel<CompanyDTO>
                    {
                        Message = $"Failed find company with ID {id}",
                        Success = false,
                    };
                }

                var errors = new List<string>();
                byte[] logoBytes;
                string base64String = null!;
                try
                {
                    logoBytes = File.ReadAllBytes(company.LogoPath);
                    base64String = Convert.ToBase64String(logoBytes);
                }
                catch (Exception ex)
                {
                    errors.Add(ex.Message);
                }
                var companyDTO = new CompanyDTO
                {
                    CompanyID = company.CompanyID,
                    CompanyName = company.CompanyName,
                    CompanyAddress = company.CompanyAddress,
                    ContactInformation = company.ContactInformation,
                    UserID = company.UserID,
                    LogoBase64 = base64String
                };

                return new ResponseModel<CompanyDTO>
                {
                    Message = "Retireved companies successfully",
                    Data = companyDTO,
                    Errors = errors
            };
            }
            catch (Exception ex)
            {
                return new ResponseModel<CompanyDTO>
                {
                    Message = "Failed to retrieve companies",
                    Success = false,
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        #endregion

        #region DeleteCompany
        public async Task<ResponseModel<string>> DeleteCompany(int id)
        {
            try
            {
                var company = await _context.Companies
                    .Where (c => c.Active)
                    .Include(pr => pr.PermissionRequest)
                    .FirstOrDefaultAsync(c => c.CompanyID == id);

                if (company == null)
                {
                    return new ResponseModel<string>
                    {
                        Message = $"Failed find company with ID {id}",
                        Success = false,
                    };
                }

                if (company.PermissionRequest != null)
                {
                    if (File.Exists(company.PermissionRequest.File_Path))
                    {
                        File.Delete(company.PermissionRequest.File_Path!);
                    }
                    _context.PermissionRequests.Remove(company.PermissionRequest);
                    await _context.SaveChangesAsync();
                }

                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();

                return new ResponseModel<string>
                {
                    Message = $"Deleted company with ID {id} successfully",
                };
            }catch (Exception ex)
            {
                return new ResponseModel<string>
                {
                    Message = "Failed to retrieve companies",
                    Success = false,
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        #endregion

        #region UpdateCompany
        public async Task<ResponseModel<CompanyDTO>> UpdateCompany(int id, UpdateCompanyDTO companyDTO)
        {
            var existingCompany = await _context.Companies
                .FirstOrDefaultAsync(c => c.CompanyID == id && c.Active);

            if (existingCompany != null)
            {
                existingCompany.CompanyName = companyDTO.CompanyName;
                existingCompany.CompanyAddress = companyDTO.CompanyAddress;
                existingCompany.ContactInformation = companyDTO.ContactInformation;

                await _context.SaveChangesAsync();

                return new ResponseModel<CompanyDTO>()
                {
                    Message = "Company updated successfully",
                    Data = new CompanyDTO
                    {
                        CompanyID = existingCompany.CompanyID,
                        CompanyName = existingCompany.CompanyName,
                        CompanyAddress = existingCompany.CompanyAddress,
                        ContactInformation = existingCompany.ContactInformation,
                        UserID = existingCompany.UserID
                    }
                };
            }
            else
            {
                return new ResponseModel<CompanyDTO>()
                {
                    Message = "Company not found",
                    Success = false
                };
            }
        }

        #endregion

        #region Get users booked with company
        public async Task<ResponseModel<IEnumerable<UserBookedWithCompanyDTO>>> GetUsersForCompanyBooking(int companyId)
        {
            var response = new ResponseModel<IEnumerable<UserBookedWithCompanyDTO>>();
            try
            {
                var users = await _context.Bookings
                    .Where(b => b.Advertisement.CompanyID == companyId)
                    .Select(b => new UserBookedWithCompanyDTO
                    {
                        UserID = b.User.Id,
                        AdId = b.AdID,
                        UserName = b.User.UserName,
                        FirstName = b.User.FirstName,
                        LastName = b.User.LastName,
                        Country = b.User.Country,
                        City = b.User.City,
                        Address = b.User.Address,
                        Email = b.User.Email
                    })
                    .Distinct()
                    .ToListAsync();

                if (!users.Any())
                {
                    return new ResponseModel<IEnumerable<UserBookedWithCompanyDTO>>
                    {
                        Success = false,
                        Message = "Company has no users booked for it"
                    };
                }

                return new ResponseModel<IEnumerable<UserBookedWithCompanyDTO>>
                {
                    Message = "Successfully retrieved users",
                    Data = users
                };
            }
            catch (Exception ex)
            {
                response.Message = "An error occurred while retrieving users for company booking";
                response.Success = false;
                response.Errors = new List<string> { ex.Message };
            }
            return response;
        }
        #endregion

        #region GetCompaniesByUserId
        public async Task<ResponseModel<IEnumerable<CompanyDTO>>> GetCompaniesByUserId(string userId)
        {
            var companies = await _context.Companies
                .Where(c => c.UserID == userId)
                .Where(c => c.Active)
                .ToListAsync();

            if (companies.Count == 0)
            {
                return new ResponseModel<IEnumerable<CompanyDTO>>
                {
                    Success = false,
                    Message = "Couldn't find any companies"
                };
            }

            var errors = new List<string>();
            var companiesDTO = new List<CompanyDTO>();
            foreach (var company in companies)
            {
                byte[] logoBytes;
                string base64String = null!;
                try
                {
                    logoBytes = File.ReadAllBytes(company.LogoPath);
                    base64String = Convert.ToBase64String(logoBytes);
                }
                catch (Exception ex)
                {
                    errors.Add(ex.Message);
                }
                var companyDTO = new CompanyDTO
                {
                    CompanyID = company.CompanyID,
                    CompanyName = company.CompanyName,
                    CompanyAddress = company.CompanyAddress,
                    ContactInformation = company.ContactInformation,
                    UserID = company.UserID,
                    LogoBase64 = base64String,
                };
                companiesDTO.Add(companyDTO);
            }

            return new ResponseModel<IEnumerable<CompanyDTO>>
            {
                Message = "Successfully retrieved companies",
                Data = companiesDTO
            };
        }
        #endregion

        #region GetCompaniesByUsername
        public async Task<ResponseModel<List<CompanyDTO>>> GetCompaniesByCompanyName(string companyname)
        {
            var response = new ResponseModel<List<CompanyDTO>>();
            try
            {
                var companies = await _context.Companies
                    .Where(c => c.CompanyName.Contains(companyname))
                    .Where(c => c.Active)
                    .ToListAsync();

                if (companies.Count == 0)
                {
                    return new ResponseModel<List<CompanyDTO>>
                    {
                        Success = false,
                        Message = "Could not find any companies"
                    };
                }

                var errors = new List<string>();
                var companiesDTO = new List<CompanyDTO>();
                foreach (var company in companies)
                {
                    byte[] logoBytes;
                    string base64String = null!;
                    try
                    {
                        logoBytes = File.ReadAllBytes(company.LogoPath);
                        base64String = Convert.ToBase64String(logoBytes);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex.Message);
                    }
                    var companyDTO = new CompanyDTO
                    {
                        CompanyID = company.CompanyID,
                        CompanyName = company.CompanyName,
                        CompanyAddress = company.CompanyAddress,
                        ContactInformation = company.ContactInformation,
                        UserID = company.UserID,
                        LogoBase64 = base64String,
                    };
                    companiesDTO.Add(companyDTO);
                }

                return new ResponseModel<List<CompanyDTO>>
                {
                    Message = "Successfully retrieved companies",
                    Data = companiesDTO
                };
            }
            catch (Exception ex)
            {
                response.Message = "An error occurred while retrieving companies";
                response.Success = false;
                response.Errors = new List<string> { ex.Message };
            }
            return response;
        }

        #endregion

        #region GetAllAdvertisementsByIDComany
        public async Task<ResponseModel<IEnumerable<AdvertisementDTO>>> GetAllAdvertisementByIDCompany(int id)
        {
            var response = new ResponseModel<IEnumerable<AdvertisementDTO>>();
            try
            {
                var ads = await _context.Advertisements
                    .Where(a => a.CompanyID == id)
                    .Select(ad => new AdvertisementDTO
                    {
                        AdID = ad.AdID,
                        CompanyName = ad.CompanyName,
                        Title = ad.Title,
                        price = ad.price,
                        Description = ad.Description,
                        ValidFrom = ad.ValidFrom,
                        ValidTo = ad.ValidTo,
                        TravelTo = ad.TravelTo,
                        TravelFrom = ad.TravelFrom,
                    }).ToListAsync();

                response.Success = true;
                response.Message = "Successfully retrieved advertisements";
                response.Data = ads;
            }
            catch (Exception ex)
            {
                response.Message = "An error occurred while retrieving advertisements";
                response.Success = false;
                response.Errors = new List<string> { ex.Message };
            }
            return response;
        }
        #endregion

        #region Company logo
        public async Task<ResponseModel<string>> SetCompanyLogo(int companyId, IFormFile image)
        {
            var company = await _context.Companies.FindAsync(companyId);
            if(company == null)
            {
                return new ResponseModel<string>
                {
                    Message = $"Failed to find company with ID {companyId}",
                    Success = false
                };
            }
            try
            {
                // Check if the logo isn't default, then delete the old logo
                if(company.LogoPath != "default_company_logo.jpg")
                {
                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, company.LogoPath);
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }

                // Change company logo to new logo
                var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".tif", ".tiff" };
                var extension = Path.GetExtension(image.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    return new ResponseModel<string>
                    {
                        Message = $"Failed to set company logo, Forbidden file type",
                        Success = false
                    };
                }
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "company_logos");
                var uniqueFileName = $"{company.CompanyID}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                company.LogoPath = filePath;
                await _context.SaveChangesAsync();
                return new ResponseModel<string>
                {
                    Message = "Successfully set the company logo"
                };
            }
            catch(Exception ex)
            {
                return new ResponseModel<string>
                {
                    Message = "Failed to set company logo, server raised exceptions",
                    Errors = new List<string>() { ex.Message },
                    Success = false
            };
            }
        }
        #endregion
    }
}
