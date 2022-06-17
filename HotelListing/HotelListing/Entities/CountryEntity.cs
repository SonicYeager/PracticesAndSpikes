namespace HotelListing.Entities;

public class CountryEntity : EntityBase
{
    public string Name { get; set; }
    public string ShortName { get; set; }
    public virtual ICollection<HotelEntity> Hotels { get; set; }
}