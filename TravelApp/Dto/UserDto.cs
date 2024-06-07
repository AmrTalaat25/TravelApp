using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TravelApp.Dto
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
    public class RegisterUserDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }

    }
    public class AuthModel
    {
        public string UserID { get; set; }
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Username { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpiresOn { get; set; }
        [JsonIgnore]
        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiration { get; set; }
    }
    public class LoginDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
    public class ManageRoleDTO
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Role { get; set; }
    }
    public class ChangePasswordDTO
    {
        public string UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
    public class UpdateUserDetailDTO
    {
        public string UserId { get; set; }
        public string NewUsername { get; set; }
        public string NewEmail { get; set; }
        public string NewFirstName { get; set; }
        public string NewLastName { get; set; }
    }
}
