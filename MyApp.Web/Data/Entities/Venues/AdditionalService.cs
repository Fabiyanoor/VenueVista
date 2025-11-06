using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Web.Data.Entities.Venues;

public class AdditionalService
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    [MaxLength(500)]
    public string? Description { get; set; }

    [Precision(18, 2)]
    public decimal Price { get; set; }

    [Required]
    public Guid VenueId { get; set; }
    public Venue? Venue { get; set; }

    public ServiceCategory Category { get; set; } // Food,Decoration,Entertainment,etc.
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum ServiceCategory
{
    Food,
    Decoration,
    Entertainment,
    Photography,
    Other
}