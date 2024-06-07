using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TravelApp.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options) 
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PermissionRequest>()
            .HasOne(pr => pr.User)
            .WithMany(u => u.PermissionRequests)
            .HasForeignKey(pr => pr.UserID)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Booking>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserID)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserID)
            .OnDelete(DeleteBehavior.NoAction);

            // one to one relationship between Company and Permission request
            modelBuilder.Entity<Company>()
            .HasOne(c => c.PermissionRequest)
            .WithOne(pr => pr.Company)
            .HasForeignKey<PermissionRequest>(pr => pr.CompanyID)
            .IsRequired(false);

            //one to one relationship between Offer and Advertisement
            modelBuilder.Entity<Offer>()
           .HasOne(o => o.Advertisement )
           .WithOne()
           .HasForeignKey<Offer>(o => o.AdID);

           //one to one relationship between Payment and Booking
            modelBuilder.Entity<Booking>()
           .HasOne(b => b.Payment)
           .WithOne(p => p.Booking)
           .HasForeignKey<Payment>(p => p.BookingID);



            // wishlist config
            modelBuilder.Entity<Wishlist>()
            .HasKey(w => w.WishlistId);

            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany(u => u.Wishlists)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.Advertisement)
                .WithMany(a => a.Wishlists)
                .HasForeignKey(w => w.AdId)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Company> Companies { get; set; }
        public DbSet<PermissionRequest> PermissionRequests { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<AdvertisementImage> AdvertisementImages { get; set; }
        public DbSet<Offer> Offers { get; set; }

        public DbSet<Wishlist> Wishlists { get; set; }

    }
}
