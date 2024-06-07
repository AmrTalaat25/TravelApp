using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelApp.Models
{
    public class Company
    {
        [Key]
        public int CompanyID { get; set; } // Primary Key
        // Foreign Key for User
        [ForeignKey("User")]
        public string UserID { get; set; } // Foreign Key
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string ContactInformation { get; set; }
        public bool Active { get; set; } = false;
        public DateTime? ApprovalDate { get; set; } = null;
        public string LogoPath { get; set; } = "default_company_logo.jpg";
        // Navigation Property
        public User User { get; set; } // Many-to-one with User
        public PermissionRequest? PermissionRequest { get; set; } // One-to-one with Permission Request Nullable
        public ICollection<Advertisement> Advertisements { get; set; } // One-to-many with Advertisement
    }
}