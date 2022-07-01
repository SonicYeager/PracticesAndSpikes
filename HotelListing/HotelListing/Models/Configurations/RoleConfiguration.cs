using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Models.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public const string USER_ROLE_NAME = "StandardUser";
        public const string ADMINISTRATOR_ROLE_NAME = "Administrator";

        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                    Name = ADMINISTRATOR_ROLE_NAME,
                    NormalizedName = "ADMINISTRATOR",
                },
                new IdentityRole
                {
                    Name = USER_ROLE_NAME,
                    NormalizedName = "STANDARDUSER",
                }
            );
        }
    }
}