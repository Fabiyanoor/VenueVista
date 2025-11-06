using MyApp.Shared.Dtos.Booking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Shared.Dtos.Venue
{
    public class VenueDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
      
        public string Status { get; set; } = string.Empty;
        public double Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public List<BookingDto> Bookings { get; set; } = new List<BookingDto>();
        public List<AdditionalServiceDto> AdditionalServices { get; set; } = new List<AdditionalServiceDto>();
        public List<VenuePackageDto> Packages { get; set; } = new List<VenuePackageDto>();
    }

    public class UpdateVenueDto : CreateVenueDto
    {
        // Inherits all properties from CreateVenueDto
    }

}