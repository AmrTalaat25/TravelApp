using Google.Apis.Auth;
using HandlebarsDotNet;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using TravelApp.Dto;
using TravelApp.Models;
using TravelApp.Models.Refresh_Token;
using TravelApp.Models.Services;
using TravelApp.Models.Services.Interfaces;

namespace TravelApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserService _service;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        public UserController(UserManager<User> userManager, IUserService service, IEmailService emailService, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            this._userManager = userManager;
            _service = service;
            _emailService = emailService;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.RegisterAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.Login(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);
            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        [HttpGet("login-google")]
        public IActionResult LogInWithGoogle()
        {
            var redirectUrl = Url.Action(nameof(HandleGoogleCallback));
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> HandleGoogleCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
                return BadRequest();

            var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
            var googleName = authenticateResult.Principal.FindFirstValue(ClaimTypes.Name);

            var username = FixUsername(googleName);
            var name = SplitNames(googleName);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = name.firstName,
                    LastName = name.lastName,
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);
            }

            var token = await _service.CreateJwtToken(user);
            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }


        private string FixUsername(string username)
        {
            return Regex.Replace(username, @"[^a-zA-Z0-9]", string.Empty);
        }

        public static (string firstName, string lastName) SplitNames(string username)
        {
            username = username.Trim();
            int lastSpaceIndex = username.LastIndexOf(' ');
            if (lastSpaceIndex == -1)
            {
                return (username, string.Empty);
            }
            string firstName = username.Substring(0, lastSpaceIndex).Trim();
            string lastName = username.Substring(lastSpaceIndex + 1).Trim();

            return (firstName, lastName);
        }

        // Get all users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            return await _service.GetAllUsers();
        }

        // Get user by username
        [HttpGet("ByUserName/{username}")]
        public ActionResult<IEnumerable<UserDTO>> GetUserByUserName(string username)
        {
            return _service.GetUserByUserName(username);
        }

        // Add role to user
        [HttpPost("addrole")]
        public async Task<IActionResult> AddRoleToUser([FromBody] ManageRoleDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.AddRoleToUser(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);
        }

        // Remove role from user
        [HttpPost("removerole")]
        public async Task<IActionResult> RemoveRoleFromUser([FromBody] ManageRoleDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.RemoveRoleFromUser(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);
        }

        // Delete user using username
        [HttpDelete("deleteuser/{userId}")]
        public async Task<IActionResult> RemoveUser(string userId)
        {
            var result = await _service.RemoveUserById(userId);
            if (result.StartsWith("User"))
            {
                return Ok(result);
            }
            else if (result.StartsWith("Failed"))
            {
                return BadRequest(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangeUserPassword([FromBody] ChangePasswordDTO model)
        {
            var result = await _service.ChangeUserPasswordById(model.UserId, model.CurrentPassword, model.NewPassword);
            if (result.StartsWith("Password"))
            {
                return Ok(result);
            }
            else if (result.StartsWith("Failed"))
            {
                return BadRequest(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPut("UpdateDetails")]
        public async Task<IActionResult> UpdateUserDetails([FromBody] UpdateUserDetailDTO model)
        {
            var result = await _service.UpdateUserDetails(model.UserId, model.NewUsername, model.NewEmail, model.NewFirstName, model.NewLastName);
            if (result.StartsWith("User details"))
            {
                return Ok(result);
            }
            else if (result.StartsWith("Failed"))
            {
                return BadRequest(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        #region Forgot Password
        [HttpPost("Forgotpassword/{email}")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email cannot be empty.");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Ok("If your email is registered, an email with instructions to reset your password has been sent.");
            }

            var newPassword = GenerateRandomPassword();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                var templatePath = Path.Combine(_webHostEnvironment.ContentRootPath, "EmailTemplates", "reset-password-template.html");
                var emailTemplate = Handlebars.Compile(System.IO.File.ReadAllText(templatePath));
                var data = new
                {
                    NewPassword = newPassword
                };
                var emailBody = emailTemplate(data);
                _emailService.SendEmail(user.Email!, "Password Reset", emailBody);

                return Ok("Password reset successful. Check your email for the new password.");
            }
            else
            {
                return BadRequest("Failed to reset password.");
            }
        }
        private string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+-=[]{}|;:,.<>?";
            var randomBytes = new byte[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            var password = new StringBuilder(length);

            foreach (byte b in randomBytes)
            {
                password.Append(validChars[b % (validChars.Length)]);
            }

            return password.ToString();
        }
        #endregion


        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var result = await _service.RefreshTokenAsync(refreshToken);

            if (!result.IsAuthenticated)
                return BadRequest(result);

            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        [HttpPost("revokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeToken model)
        {
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required!");

            var result = await _service.RevokeTokenAsync(token);

            if (!result)
                return BadRequest("Token is invalid!");

            return Ok();
        }

        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        #region Email Confirm
        [HttpPost("SendConfirmationEmail")]
        public async Task<IActionResult> SendConfirmEmailToken(string userId)
        {
            var result = await _service.SendConfirmationEmailToken(userId);
            if (result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("ConfirmUserEmail")]
        public async Task<IActionResult> ConfirmUserEmailWithToken(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User Not Found");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return (BadRequest(result.Errors));
            }

            return Ok("Confirmed Email Successfully");
        }
        #endregion

        #region Email Change
        [HttpPost("SendChangeEmailToken")]
        public async Task<IActionResult> SendChangeEmailToken(string userId, string newEmail)
        {
            var result = await _service.SendEmailChangeToken(userId, newEmail);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("ChangeEmail")]
        public async Task<IActionResult> ChangeUserEmail(string userId, string newEmail, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound($"User with Id {userId} Not Found");
            }

            if (await _userManager.FindByEmailAsync(newEmail) is not null)
            {
                return BadRequest("This email is already registered with a different user");
            }

            var result = await _userManager.ChangeEmailAsync(user, newEmail, token);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok("Changed Email Successfully");
        }
        #endregion
    }
}