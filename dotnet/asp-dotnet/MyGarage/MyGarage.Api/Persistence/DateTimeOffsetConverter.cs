using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MyGarage.Api.Persistence;

public class DateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
{
    public DateTimeOffsetConverter()
        : base(static d => d.ToUniversalTime(), static d => d.ToUniversalTime())
    {
    }
}