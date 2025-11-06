using MyApp.Shared.Dtos.Booking;
using System.Collections.Generic;

namespace MyApp.Shared.Dtos.User;

public class UserDetailsDto : UserSummaryDto
{
    public List<BookingWithUserDto> Bookings { get; set; } = new List<BookingWithUserDto>();
}


    public class UpdateUserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string Role { get; set; }
    }
