using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TravelApp.Models
{
    public class Wishlist
    {
        [Key]
        public int WishlistId { get; set; }
        [ForeignKey("UserId")]
        public string UserId { get; set; }
        [ForeignKey("AdId")]
        public int AdId { get; set; }
        public User User { get; set; }
        public Advertisement Advertisement { get; set; }
    }
}
