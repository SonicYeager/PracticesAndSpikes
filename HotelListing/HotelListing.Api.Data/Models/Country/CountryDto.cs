using HotelListing.Api.Data.Models.Hotel;
using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.Data.Models.Country
{
    public class CountryDto : BaseCountryDto
    {
        [Required]
        public int Id { get; set; }

        public IEnumerable<HotelDto> Hotels { get; set; }
    }
}