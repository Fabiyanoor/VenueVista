using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Shared.Dtos.Venue
{
    public class AdditionalServiceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public Guid VenueId { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public ServiceCategory Category { get; set; }
    }

    public class CreateAdditionalServiceDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public Guid VenueId { get; set; }
        public ServiceCategory Category { get; set; }
    }

    public enum ServiceCategory
    {
        Food,
        Decoration,
        Entertainment,
        Photography,
        Other
    }
}