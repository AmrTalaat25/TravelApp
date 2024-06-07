using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelApp.Models
{
    public class AdvertisementImage
    {
        [Key]
        public int ImageID { get; set; } // Primary Key
        [ForeignKey("Advertisement")]
        public int AdID { get; set; } // Foreign Key
        public string? Imagepath { get; set; }

        // Navigation Property
        public Advertisement Advertisement { get; set; } // Many-to-one with Advertisement
    }
}
