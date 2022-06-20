using System.ComponentModel.DataAnnotations;

namespace HotelListing.Models.Country
{
    public class GetCountryDto : BaseCountryDto
    {
        [Required]
        public int Id { get; set; }
    }
}