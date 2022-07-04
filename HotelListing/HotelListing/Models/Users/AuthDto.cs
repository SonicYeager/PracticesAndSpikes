namespace HotelListing.Models.Users
{
    public class AuthDto
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}