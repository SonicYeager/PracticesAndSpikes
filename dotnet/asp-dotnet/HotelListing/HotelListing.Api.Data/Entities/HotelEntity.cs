﻿using System.ComponentModel.DataAnnotations.Schema;

namespace HotelListing.Api.Data.Entities;

public sealed class HotelEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public double Rating { get; set; }
    [ForeignKey(nameof(CountryId))]
    public int CountryId { get; set; }
    public CountryEntity Country { get; set; }
}