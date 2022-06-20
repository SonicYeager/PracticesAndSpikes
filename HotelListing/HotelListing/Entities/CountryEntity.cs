using System.ComponentModel.DataAnnotations;

namespace HotelListing.Entities;

public sealed class CountryEntity
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public string ShortName { get; set; }
    public ICollection<HotelEntity> Hotels { get; set; }
}