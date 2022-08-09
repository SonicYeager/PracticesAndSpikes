using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.Data.Models.Country
{
    public class GetCountryDto : BaseCountryDto
    {
        [Required]
        public int Id { get; set; }
    }
}