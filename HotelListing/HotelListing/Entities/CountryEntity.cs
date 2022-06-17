namespace HotelListing.Entities;

public sealed class CountryEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public ICollection<HotelEntity> Hotels { get; set; }
}