using MyApp.Shared.Dtos.Venue;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Shared.Dtos
{
    public class CreateVenueDto
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Type { get; set; } = string.Empty;




        [Required, MaxLength(50)]
        public string Status { get; set; } = "Available";

        public List<string> Services { get; set; } = new();
        public List<string> ImageUrls { get; set; } = new();
    }

    public class UpdateVenueDto : CreateVenueDto
    {
        // Inherits all properties from CreateVenueDto
    }
}