using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TravelApp.Models;

namespace TravelApp.Dto
{
    public class CreatePermissionRequestDTO
    {
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? CompanyName { get; set; }
        [Required]
        public string? CompanyAddress { get; set; }
        [Required]
        public string? ContactInformation { get; set; }
        [Required]
        public IFormFile? File { get; set; }
    }
    public class PermissionRequestDTO
    {
        public int RequestID { get; set; }
        public string? UserID { get; set; }
        public int CompanyID { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public string? ContactInformation { get; set; }
    }
    public class UpdatePermissionRequestDTO
    {
        public string? Description { get; set; }
        public string? CompanyName { get;set; }
        public string? CompanyAddress { get; set; }
        public string? ContactInformation { get; set;}
        public IFormFile? File { get;set; }
    }
    public class PermissionRequestDetailedDto
    {
        public int RequestID { get; set; }
        public string? UserID { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public int CompanyID { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public string? ContactInformation { get; set; }
    }
}
