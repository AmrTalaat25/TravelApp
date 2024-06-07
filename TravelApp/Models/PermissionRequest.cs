using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelApp.Models
{
    public class PermissionRequest
    {
        [Key]
        public int RequestID { get; set; } // Primary Key
        [ForeignKey("User")]
        public string UserID { get; set; } // Foreign Key
        [ForeignKey("Company")]
        public int CompanyID { get; set; } // Foreign Key
        public string Description { get; set; }
        public string Status { get; set; }
        public string? File_Path { get; set; }
        public User User { get; set; } // Many-to-one with Users
        public Company Company { get; set; } // One-to-one with Company
    }
}
