using Microsoft.AspNetCore.Identity;

namespace HotelListing.Entities
{
    public class HotelListingUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}