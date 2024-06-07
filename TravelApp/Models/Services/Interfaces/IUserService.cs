using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using TravelApp.Dto;

namespace TravelApp.Models.Services.Interfaces
{
    public interface IUserService
    {
        Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers();
        public ActionResult<IEnumerable<UserDTO>> GetUserByUserName(string username);
        Task<AuthModel> RegisterAsync(RegisterUserDTO model);
        Task<AuthModel> Login(LoginDto loginDto);
        Task<string> AddRoleToUser(ManageRoleDTO addRoleDto);
        Task<string> RemoveRoleFromUser(ManageRoleDTO addRoleDto);
        Task<string> RemoveUserById(string username);
        Task<string> ChangeUserPasswordById(string userId, string currentPassword, string newPassword);
        Task<string> UpdateUserDetails(string userId, string newUsername, string newEmail, string newFirstName, string newLastName);
        Task<JwtSecurityToken> CreateJwtToken(User user);
        Task<AuthModel> RefreshTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);
        Task<ResponseModel<string>> SendConfirmationEmailToken(string userId);
        Task<ResponseModel<string>> SendEmailChangeToken(string userId, string newEmail);
    }
}