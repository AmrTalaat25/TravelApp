using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelApp.Models
{
    public class Review
    {
        [Key]
        public int ReviewID { get; set; } // Primary Key
        [ForeignKey("Advertisement")]
        public int AdID { get; set; } // Foreign Key
        [ForeignKey("User")]
        public string UserID { get; set; } // Foreign Key
        public string Username { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime DatePosted { get; set; }

        // Navigation Properties
        public Advertisement Advertisement { get; set; } // Many-to-one with Advertisement
        public User User { get; set; } // Many-to-one with User
    }
}
