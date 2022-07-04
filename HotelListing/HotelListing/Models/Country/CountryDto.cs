using HotelListing.Models.Hotel;
using System.ComponentModel.DataAnnotations;

namespace HotelListing.Models.Country
{
    public class CountryDto : BaseCountryDto
    {
        [Required]
        public int Id { get; set; }

        public IEnumerable<HotelDto> Hotels { get; set; }
    }
}