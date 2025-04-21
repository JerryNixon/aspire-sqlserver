using AlphaDynamics.Poco;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AlphaDynamics.SqlRepository;

public class AlphaDynamicsContext : DbContext
{
    public DbSet<Crew> Crews => Set<Crew>();
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<Operation> Operations => Set<Operation>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    public AlphaDynamicsContext(DbContextOptions<AlphaDynamicsContext> options)
    : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Operation>().HasData(
            new Operation { Id = 1, Name = "CheckOut" },
            new Operation { Id = 2, Name = "CheckIn" }
        );

        modelBuilder.Entity<Crew>().HasData(
            new Crew { Id = 1, Name = "Janet Vega" },
            new Crew { Id = 2, Name = "Miles Ortez" },
            new Crew { Id = 3, Name = "Tessa Qin" }
        );

        modelBuilder.Entity<Equipment>().HasData(
            new Equipment { Id = 1, Name = "EVA Suit" },
            new Equipment { Id = 2, Name = "Toolkit Alpha" },
            new Equipment { Id = 3, Name = "Thermal Scanner" }
        );
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAlphaDynamicsRepository(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AlphaDynamicsContext>(options =>
            options.UseSqlServer(connectionString));
        return services;
    }
}
