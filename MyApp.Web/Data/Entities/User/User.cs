using MyApp.Web.Data.Entities.Bookings;
using System.ComponentModel.DataAnnotations;

public class User
{
 
        public Guid Id { get; set; }
        [Required, MaxLength(100)]
        public string? Name { get; set; }
        [Required, EmailAddress, MaxLength(200)]
        public string? Email { get; set; }
        [Phone, MaxLength(20)]
        public string? ContactNumber { get; set; }
        [Required]
        public string? PasswordHash { get; set; }
        [Required, MaxLength(50)]
        public string Role { get; set; } = "User"; // Default to "User", can be "Admin", etc.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    
}
