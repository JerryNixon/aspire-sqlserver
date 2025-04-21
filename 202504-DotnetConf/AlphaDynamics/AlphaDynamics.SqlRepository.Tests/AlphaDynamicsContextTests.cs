using AlphaDynamics.Poco;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

namespace AlphaDynamics.SqlRepository.Tests;

public class OperationTests
{
    private DbContextOptions<AlphaDynamicsContext> CreateOptions(string dbName) =>
        new DbContextOptionsBuilder<AlphaDynamicsContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

    [Fact]
    public void AddTransactions_ShouldInsertMultipleAndLinkToCrewAndEquipment()
    {
        // Arrange
        var options = CreateOptions(Guid.NewGuid().ToString());

        using var context = new AlphaDynamicsContext(options);
        context.Database.EnsureCreated();

        var crew1 = new Crew { Id = 10, Name = "Janet" };
        var crew2 = new Crew { Id = 11, Name = "Miles" };
        var eq1 = new Equipment { Id = 20, Name = "Toolkit" };
        var eq2 = new Equipment { Id = 21, Name = "Oxygen Tank" };

        context.Crews.AddRange(crew1, crew2);
        context.Equipment.AddRange(eq1, eq2);
        context.SaveChanges();

        // Act
        context.Transactions.AddRange(
            new Transaction { CrewId = 10, EquipmentId = 20, Date = DateTime.UtcNow, OperationId = 1 },
            new Transaction { CrewId = 11, EquipmentId = 21, Date = DateTime.UtcNow, OperationId = 1 },
            new Transaction { CrewId = 10, EquipmentId = 20, Date = DateTime.UtcNow.AddHours(2), OperationId = 2 }
        );
        context.SaveChanges();

        // Assert
        context.Transactions.Should().HaveCount(3);
        context.Transactions.Count(t => t.OperationId == 1).Should().Be(2);
        context.Transactions.Count(t => t.CrewId == 10 && t.EquipmentId == 20).Should().Be(2);
    }

    [Fact]
    public void GetOperations_ShouldReturnSeededCheckInAndCheckOut()
    {
        // Arrange
        var options = CreateOptions("SeededOpsDb");
        using var context = new AlphaDynamicsContext(options);
        context.Database.EnsureCreated();

        // Act
        var operations = context.Operations.ToList();

        // Assert
        operations.Should().HaveCount(2);
        operations.Should().Contain(o => o.Id == 1 && o.Name == "CheckOut");
        operations.Should().Contain(o => o.Id == 2 && o.Name == "CheckIn");
    }

    [Fact]
    public void AddOperation_ShouldAddCustomOperation()
    {
        // Arrange
        var options = CreateOptions("AddOpDb");
        using var context = new AlphaDynamicsContext(options);

        var op = new Operation { Id = 3, Name = "Maintenance" };

        // Act
        context.Operations.Add(op);
        context.SaveChanges();

        // Assert
        context.Operations.Should().ContainSingle(o => o.Id == 3 && o.Name == "Maintenance");
    }

    [Fact]
    public void AddOperation_WithDuplicateId_ShouldSucceedInMemory()
    {
        // Arrange
        var options = CreateOptions("DupIdDb");
        using var context = new AlphaDynamicsContext(options);
        var op = new Operation { Id = 1, Name = "DuplicateCheckOut" };

        // Act / Assert
        context.Operations.Add(op);
        context.Invoking(c => c.SaveChanges())
               .Should().NotThrow("because InMemory does not enforce primary key constraints");
    }
}