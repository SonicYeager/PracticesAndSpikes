using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.Data.Models.Users
{
    public class UserDto : LoginDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }
}