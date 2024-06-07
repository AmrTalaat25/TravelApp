using HandlebarsDotNet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using TravelApp.Dto;
using TravelApp.Models.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TravelApp.Models.Services
{
    public class PermissionRequestService : IPermissionRequestService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        public PermissionRequestService(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<User> userManager, IEmailService emailService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _emailService = emailService;
        }

        #region Create Permission Request
        public async Task<ResponseModel<PermissionRequestDTO>> CreatePermissionRequest(CreatePermissionRequestDTO request, string userId)
        {
            // first upload PDF document
            if (request.File == null || request.File.Length == 0)
            {
                return new ResponseModel<PermissionRequestDTO>
                {
                    Message = "Failed to upload",
                    Success = false
                };
            }
            var allowedExtensions = new[] { ".pdf" };
            var extension = Path.GetExtension(request.File.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                return new ResponseModel<PermissionRequestDTO>
                {
                    Message = "Only pdf allowed",
                    Success = false
                };
            }
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "pdf_files");
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }

            // second create the company
            var company = new Company
            {
                UserID = userId!,
                CompanyName = request.CompanyName!,
                CompanyAddress = request.CompanyAddress!,
                ContactInformation = request.ContactInformation!
            };
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();

            // third create the permission request
            var permissionRequset = new PermissionRequest
            {
                UserID = userId!,
                CompanyID = company.CompanyID,
                Description = request.Description!,
                Status = "pending",
                File_Path = filePath,
            };
            await _context.PermissionRequests.AddAsync(permissionRequset);
            await _context.SaveChangesAsync();

            // finally send the email and return the newly created request and company data
            var user = await _userManager.FindByIdAsync(userId);
            var templatePath = Path.Combine(_webHostEnvironment.ContentRootPath, "EmailTemplates", "new-permission-request-template.html");
            var emailTemplate = Handlebars.Compile(File.ReadAllText(templatePath));
            var data = new
            {
                FirstName = user!.FirstName,
                LastName = user.LastName,
                CompanyName = company.CompanyName,
                CompanyAddress = company.CompanyAddress
            };
            var emailBody = emailTemplate(data);
            _emailService.SendEmail(user!.Email!, "Request Company", emailBody);

            return new ResponseModel<PermissionRequestDTO>
            {
                Message = "Created Request Successfully",
                Data = new PermissionRequestDTO
                {
                    RequestID = permissionRequset.RequestID,
                    UserID = userId,
                    CompanyID = company.CompanyID,
                    Description = permissionRequset.Description,
                    Status = permissionRequset.Status,
                    CompanyName = company.CompanyName,
                    CompanyAddress = company.CompanyAddress,
                    ContactInformation = company.ContactInformation,
                }
            };
        }
        #endregion

        #region Get All Permission Requests
        public async Task<ResponseModel<IEnumerable<PermissionRequestDTO>>> GetPermissionRequests()
        {
            try
            {
                var permissionRequests = await _context.PermissionRequests
                    .Include(r => r.Company)
                    .Select(r => new PermissionRequestDTO
                    {
                        RequestID = r.RequestID,
                        UserID = r.UserID,
                        CompanyID = r.CompanyID,
                        Description = r.Description,
                        Status = r.Status,
                        CompanyName = r.Company.CompanyName,
                        CompanyAddress = r.Company.CompanyAddress,
                        ContactInformation = r.Company.ContactInformation
                }).ToListAsync();
                return new ResponseModel<IEnumerable<PermissionRequestDTO>>
                {
                    Message = "Retrieved Requests Successfully",
                    Data = permissionRequests
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<IEnumerable<PermissionRequestDTO>>
                {
                    Message = "Service failed to retrieve permission requests",
                    Errors = new List<string> { ex.Message },
                    Success = false
                };
            }
        }
        #endregion

        #region Get Permission Request by ID
        public async Task<ResponseModel<PermissionRequestDetailedDto>> GetPermissionRequestsById(int requestID)
        {
            var request = await _context.PermissionRequests
            .Include(c => c.Company)
            .Include(u => u.User)
            .FirstOrDefaultAsync(pr => pr.RequestID == requestID);
            if (request == null)
            {
                return new ResponseModel<PermissionRequestDetailedDto>
                {
                    Message = $"Could not find request with id {requestID}",
                    Success = false
                };
            }

            var data = new PermissionRequestDetailedDto
            {
                RequestID = request.RequestID,
                UserID = request.UserID,
                Username = request.User.UserName,
                Email = request.User.Email,
                CompanyID = request.Company.CompanyID,
                Description = request.Description,
                Status = request.Status,
                CompanyName = request.Company.CompanyName,
                CompanyAddress = request.Company.CompanyAddress,
                ContactInformation = request.Company.ContactInformation
            };

            return new ResponseModel<PermissionRequestDetailedDto>
            {
                Message = "Retrieved permission request successfully!",
                Data = data
            };
        }
        #endregion

        #region Get Permission Requests by UserID
        public async Task<ResponseModel<List<PermissionRequestDetailedDto>>> GetPermissionRequestsByUser(string userID)
        {
            var requests = await _context.PermissionRequests
                .Where(pr => pr.UserID == userID)
                .Include(r => r.User)
                .Include(r => r.Company)
                .Select(r => new PermissionRequestDetailedDto
            {
                RequestID = r.RequestID,
                UserID = r.UserID,
                Username = r.User.UserName,
                Email = r.User.Email,
                CompanyID = r.Company.CompanyID,
                Description = r.Description,
                Status = r.Status,
                CompanyName = r.Company.CompanyName,
                CompanyAddress = r.Company.CompanyAddress,
                ContactInformation = r.Company.ContactInformation
            }).ToListAsync();
            if (requests.Count == 0)
            {
                return new ResponseModel<List<PermissionRequestDetailedDto>>
                {
                    Message = "User has no permission requests",
                    Success = false
                };
            }

            return new ResponseModel<List<PermissionRequestDetailedDto>>
            {
                Message = "Retrieved user permission requests successfully",
                Data = requests
            };
        }
        #endregion

        #region Get Permission Request Document
        public async Task<ResponseModel<byte[]>> GetPermissionRequestDocument(int requestID)
        {
            try
            {
                var permissionRequest = await _context.PermissionRequests.FindAsync(requestID);

                if (permissionRequest == null)
                {
                    return new ResponseModel<byte[]>
                    {
                        Message = $"Could not find request with ID {requestID}",
                        Success = false
                    };
                }

                if (!File.Exists(permissionRequest.File_Path))
                {
                    return new ResponseModel<byte[]>
                    {
                        Message = $"Could not find document for permission request with ID {requestID}",
                        Success = false
                    };
                }
                byte[] document = await File.ReadAllBytesAsync(permissionRequest.File_Path);
                return new ResponseModel<byte[]>
                {
                    Message = "Retrieved Document Successfully",
                    Data = document
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<byte[]>
                {
                    Message = "Something went wrong while retrieving the document",
                    Success = false,
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        #endregion

        #region Approve Permision Request
        public async Task<ResponseModel<string>> ApprovePermissionRequest(int requestID)
        {
            // Check if request can be accepted or rejected
            var request = await _context.PermissionRequests.FindAsync(requestID);
            if (request == null)
            {
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = $"Could not find request with ID {requestID}"
                };
            }
            if (request.Status == "approved")
            {
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = $"request with ID {requestID} is already approved"
                };
            }

            // Give user the role CompanyOwner if they don't have it
            var user = await _userManager.FindByIdAsync(request.UserID);
            if (user == null)
            {
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = $"could not find User with ID {request.UserID}"
                };
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Contains("CompanyOwner"))
            {
                var result = await _userManager.AddToRoleAsync(user, "CompanyOwner");
                if (!result.Succeeded)
                {
                    return new ResponseModel<string>
                    {
                        Success = false,
                        Message = $"Failed to change user role to CompanyOwner",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }
            }

            // Chnage request status to approved
            request.Status = "approved";
            await _context.SaveChangesAsync();

            // set company Active to true and store approval date
            var company = await _context.Companies.FindAsync(request.CompanyID);
            if (company == null) 
            {
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = $"Company with id {request.CompanyID} for  request with id {request.RequestID} does not exist"
                };
            }
            company.Active = true;
            company.ApprovalDate = DateTime.Now;
            await _context.SaveChangesAsync();

            // Send Approval Email
            var templatePath = Path.Combine(_webHostEnvironment.ContentRootPath, "EmailTemplates", "approve-permission-request-template.html");
            var emailTemplate = Handlebars.Compile(File.ReadAllText(templatePath));
            var data = new
            {
                FirstName = user!.FirstName,
                LastName = user.LastName,
                CompanyName = company.CompanyName
            };
            var emailBody = emailTemplate(data);
            _emailService.SendEmail(user!.Email!, "Your Request Has Been Approved!", emailBody);

            return new ResponseModel<string>
            {
                Message = $"Approved request with ID {requestID} Successfully"
            };
        }
        #endregion

        #region Reject Permission Request with request ID
        public async Task<ResponseModel<string>> RejectPermissionRequest(int requestID)
        {
            try
            {
                // Get the request from the ID
                var request = await _context.PermissionRequests.FindAsync(requestID);
                var errors = new List<string>();
                if (request == null)
                {
                    return new ResponseModel<string>
                    {
                        Success = false,
                        Message = $"Could not find request with ID {requestID}"
                    };
                }
                if (request.Status == "approved")
                {
                    return new ResponseModel<string>
                    {
                        Success = false,
                        Message = $"Cannot reject the request {requestID} because it has already been approved"
                    };
                }

                // Delete the document related to the request
                if (File.Exists(request.File_Path))
                {
                    File.Delete(request.File_Path!);
                }
                else
                {
                    errors.Add($"Couldn't find file at {request.File_Path}");
                }

                // Delete the request itself
                _context.PermissionRequests.Remove(request);
                _context.SaveChanges();

                // Delete the company related to the permission request
                var company = await _context.Companies.FindAsync(request.CompanyID);
                if (company == null)
                {
                    errors.Add($"Couldn't find company with ID {request.CompanyID}");
                }
                else
                {
                    _context.Companies.Remove(company);
                    _context.SaveChanges();
                }

                // Send rejection email
                var user = await _userManager.FindByIdAsync(request.UserID);
                var templatePath = Path.Combine(_webHostEnvironment.ContentRootPath, "EmailTemplates", "reject-permission-request-template.html");
                var emailTemplate = Handlebars.Compile(File.ReadAllText(templatePath));
                var data = new
                {
                    FirstName = user!.FirstName,
                    LastName = user.LastName
                };
                var emailBody = emailTemplate(data);
                _emailService.SendEmail(user!.Email!, "Your Request Has Been Rejected.", emailBody);

                return new ResponseModel<string>
                {
                    Errors = errors,
                    Message = "Rejected Permission Request Successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<string>
                {
                    Errors = new List<string> { ex.Message },
                    Message = "Could not reject permission request",
                    Success = false
                };
            }
        }

        #endregion

        #region Update Pending Permission Request
         public async Task<ResponseModel<string>> UpdatePermissionRequest(int requestID, UpdatePermissionRequestDTO request)
         {
            try
            {
                // Get the permission request from the ID
                var permissionRequest = await _context.PermissionRequests.FindAsync(requestID);
                if (permissionRequest == null)
                {
                    return new ResponseModel<string>
                    {
                        Success = false,
                        Message = $"Could not find permission request"
                    };
                }
                var company = await _context.Companies.FindAsync(permissionRequest.CompanyID);
                if (permissionRequest == null)
                {
                    return new ResponseModel<string>
                    {
                        Success = false,
                        Message = $"Could not find company"
                    };
                }

                if (permissionRequest.Status == "approved")
                {
                    return new ResponseModel<string>
                    {
                        Success = false,
                        Message = $"Cannot change approved permission request data"
                    };
                }

                // Update the file
                if (File.Exists(permissionRequest.File_Path) && request.File != null)
                {
                    File.Delete(permissionRequest.File_Path);
                    var allowedExtensions = new[] { ".pdf" };
                    var extension = Path.GetExtension(request.File!.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension))
                    {
                        return new ResponseModel<string>
                        {
                            Message = "Only pdf allowed",
                            Success = false
                        };
                    }
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "pdf_files");
                    var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.File.CopyToAsync(stream);
                    }
                    permissionRequest.File_Path = filePath;
                }

                // Update the company and request
                if (request.Description != null) { permissionRequest.Description = request.Description; }
                if (request.CompanyName != null) { company!.CompanyName = request.CompanyName; }
                if (request.CompanyAddress != null) { company!.CompanyAddress = request.CompanyAddress; }
                if (request.ContactInformation != null) { company!.ContactInformation = request.ContactInformation; }

                // Save changes
                await _context.SaveChangesAsync();

                return new ResponseModel<string>
                {
                    Success = true,
                    Message = $"Permission request with ID {requestID} updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = $"An error occurred while updating the permission request: {ex.Message}",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        #endregion
    }
}