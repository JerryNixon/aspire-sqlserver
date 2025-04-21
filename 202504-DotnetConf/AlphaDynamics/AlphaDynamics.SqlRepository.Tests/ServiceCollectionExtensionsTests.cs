using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

namespace AlphaDynamics.SqlRepository.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddAlphaDynamicsRepository_ShouldRegisterDbContext()
    {
        // Arrange
        var services = new ServiceCollection();
        var connectionString = "Server=localhost;Database=FakeDb;User Id=sa;Password=P@ssw0rd!;";

        // Act
        services.AddAlphaDynamicsRepository(connectionString);
        var provider = services.BuildServiceProvider();
        var context = provider.GetService<AlphaDynamicsContext>();

        // Assert
        context.Should().NotBeNull();
        context.Should().BeOfType<AlphaDynamicsContext>();
    }
}