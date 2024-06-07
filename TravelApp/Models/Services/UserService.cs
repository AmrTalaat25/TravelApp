using HandlebarsDotNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Cryptography;
using Newtonsoft.Json.Linq;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http.ModelBinding;
using TravelApp.Dto;
using TravelApp.Models.Refresh_Token;
using TravelApp.Models.Services.Interfaces;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static TravelApp.Models.Services.UserService;

namespace TravelApp.Models.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration Configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailService _emailService;
        public UserService(UserManager<User> userManager, ApplicationDbContext context, IConfiguration configuration, RoleManager<IdentityRole> roleManager, IWebHostEnvironment webHostEnvironment, IEmailService emailService)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
            Configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _emailService = emailService;
        }
        #region Register User
        public async Task<AuthModel> RegisterAsync(RegisterUserDTO model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel
                { Message = "Email is already registered!" };

            var user = new User
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Country = model.Country,
                City = model.City,
                Address = model.Address
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new AuthModel { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, "User");

            return new AuthModel
            {
                Email = user.Email,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Username = user.UserName,
            };
        }
        #endregion

        #region Login User
        public async Task<AuthModel> Login(LoginDto model)
        {
            var authModel = new AuthModel();

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);

            // Get user roles
            var rolesList = await _userManager.GetRolesAsync(user);

            // Populate AuthModel
            authModel = new AuthModel
            {
                UserID = user.Id,
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Country = user.Country,
                City = user.City,
                Address = user.Address,
                Roles = rolesList.ToList(), // Convert to List<string>
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName
            };

            // Check for active refresh token
            var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
            if (activeRefreshToken != null)
            {
                authModel.RefreshToken = activeRefreshToken.Token;
                authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                // Generate and add refresh token
                var refreshToken = GenerateRefreshToken();
                authModel.RefreshToken = refreshToken.Token;
                authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }

            return authModel;
        }
        #endregion



        #region Get All Users
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            var users = await _context.Users.Select(x => new UserDTO
            {
                Id = x.Id,
                UserName = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Country = x.Country,
                City = x.City,
                Address = x.Address,
                Email = x.Email,
                Roles = _userManager.GetRolesAsync(x).Result.ToList()
            }).ToListAsync();
            return users;
        }
        #endregion

        #region Get User by ID
        public ActionResult<IEnumerable<UserDTO>> GetUserByUserName(string username)
        {
            var normalizedUsername = username.ToUpper();

            var users = _context.Users
                .Where(x => x.NormalizedUserName.Contains(normalizedUsername))
                .Select(c => new UserDTO
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    Roles = _userManager.GetRolesAsync(c).Result.ToList()
                }).ToList();

            if (users.Count == 0)
            {
                throw new Exception("User not found");
            }

            return users;
        }

        #endregion

        #region Public Method Create JWT Token
        public async Task<JwtSecurityToken> CreateJwtToken(User user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var jwtSettings = Configuration.GetSection("JWT");
            var signingCredentials = new SigningCredentials(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"])),
        SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: jwtSettings["ValidIssuer"],
                audience: jwtSettings["ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["DurationInMinutes"])),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        #endregion

        #region Add Role to User
        public async Task<string> AddRoleToUser(ManageRoleDTO model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return "Invalid user ID or Role";

            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "User already assigned to this role";

            var result = await _userManager.AddToRoleAsync(user, model.Role);

            return result.Succeeded ? string.Empty : "Something went wrong";
        }
        #endregion

        #region Remove Role From User
        public async Task<string> RemoveRoleFromUser(ManageRoleDTO model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return "Invalid user ID or Role";

            if (model.Role == "User")
                return "Cannot remove the default 'User' role from the user";

            if (!await _userManager.IsInRoleAsync(user, model.Role))
                return "User is not assigned to this role";

            var result = await _userManager.RemoveFromRoleAsync(user, model.Role);

            return result.Succeeded ? string.Empty : "Something went wrong";
        }


        #endregion
        
        #region Delete User By Id
        public async Task<string> RemoveUserById(string userId)
        {
            var userToDelete = await _userManager.FindByIdAsync(userId);
            if (userToDelete != null)
            {
                var result = await _userManager.DeleteAsync(userToDelete);
                if (result.Succeeded)
                {
                    return $"User '{userId}' has been successfully deleted.";
                }
                else
                {
                    // If there are any specific errors, concatenate them into the return message
                    var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                    return $"Failed to delete user '{userId}': {errorMessage}";
                }
            }
            else
            {
                return $"User '{userId}' was not found.";
            }
        }
        #endregion

        #region Change User Password By Id
        public async Task<string> ChangeUserPasswordById(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                if (result.Succeeded)
                {
                    return $"Password changed successfully for user with ID '{userId}'.";
                }
                else
                {
                    var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                    return $"Failed to change password for user with ID '{userId}': {errorMessage}";
                }
            }
            else
            {
                return $"User with ID '{userId}' was not found.";
            }
        }
        #endregion

        #region Update User Details Using Id
        public async Task<string> UpdateUserDetails(string userId, string newUsername, string newEmail, string newFirstName, string newLastName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.UserName = newUsername;
                user.Email = newEmail;
                user.FirstName = newFirstName;
                user.LastName = newLastName;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return $"User details updated successfully for user with ID '{userId}'.";
                }
                else
                {
                    var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                    return $"Failed to update user details for user with ID '{userId}': {errorMessage}";
                }
            }
            else
            {
                return $"User with ID '{userId}' was not found.";
            }
        }
        #endregion

        #region Refresh Token
        public async Task<AuthModel> RefreshTokenAsync(string token)
        {
            var authModel = new AuthModel();

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                authModel.Message = "Invalid token";
                return authModel;
            }

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
            {
                authModel.Message = "Inactive token";
                return authModel;
            }

            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var jwtToken = await CreateJwtToken(user);
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            var roles = await _userManager.GetRolesAsync(user);
            authModel.Roles = roles.ToList();
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

            return authModel;
        }
        #endregion

        #region Revoke Token
        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                return false;

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return true;
        }
        #endregion

        #region Private Method GenerateRefreshToken
        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };
        }
        #endregion


        #region Confirmation Email Sender
        public async Task<ResponseModel<string>> SendConfirmationEmailToken(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ResponseModel<string>
                    {
                        Success = false,
                        Message = $"Failed to find user with Id {userId}"
                    };
                }

                if (user!.EmailConfirmed)
                {
                    return new ResponseModel<string>
                    {
                        Success = false,
                        Message = $"Failed, User already confirmed."
                    };
                }

                var token = await _userManager.GenerateUserTokenAsync(user!, "EmailTokenProvider", "EmailConfirmation");
                var templatePath = Path.Combine(_webHostEnvironment.ContentRootPath, "EmailTemplates", "confirm-template.html");
                var emailTemplate = Handlebars.Compile(File.ReadAllText(templatePath));
                var data = new
                {
                    Token = token
                };
                var emailBody = emailTemplate(data);
                _emailService!.SendEmail(user!.Email!, "Confirm Email", emailBody);
                return new ResponseModel<string>
                {
                    Message = $"Email sent to {user.Email!} successfully!",
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<string>
                {
                    Message = "Could not send email",
                    Errors = new List<string>()
                    {
                        ex.Message
                    },
                    Success = false,
                };
            }
        }
        #endregion

        #region Change Email Token Sender
        public async Task<ResponseModel<string>> SendEmailChangeToken(string userId, string newEmail)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = $"Failed to find user with Id {userId}"
                };
            }
            var token = await _userManager.GenerateUserTokenAsync(user, "EmailTokenProvider", "ChangeEmail:" + newEmail);
            try
            {
                var templatePath = Path.Combine(_webHostEnvironment.ContentRootPath, "EmailTemplates", "change-template.html");
                var emailTemplate = Handlebars.Compile(File.ReadAllText(templatePath));
                var data = new
                {
                    Token = token
                };
                var emailBody = emailTemplate(data);
                _emailService!.SendEmail(newEmail, "Change Email", emailBody);
                return new ResponseModel<string>
                {
                    Message = $"Email sent to {newEmail} successfully!",
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = "Failed to send email",
                    Errors = new List<string>()
                    {
                        ex.Message
                    },
                };
            }
        }
        #endregion
    }
}
