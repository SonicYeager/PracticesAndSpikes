using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FindACoach.Persistence;

public sealed class FindACoachDbContextFactory : IDesignTimeDbContextFactory<FindACoachDbContext>
{
    /// <inheritdoc />
    public FindACoachDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FindACoachDbContext>();

        // Hardcoded ConnectionString für MariaDB
        const string connectionString = "Server=localhost;Port=3306;Database=FindACoach;User=root;Password=my-secret;";

        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new(optionsBuilder.Options);
    }

}