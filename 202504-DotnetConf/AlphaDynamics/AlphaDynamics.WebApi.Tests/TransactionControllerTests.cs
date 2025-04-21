using AlphaDynamics.Poco;
using AlphaDynamics.SqlRepository;
using AlphaDynamics.WebApi.Controllers;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlphaDynamics.WebApi.Tests;

public class TransactionControllerTests
{
    private AlphaDynamicsContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AlphaDynamicsContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AlphaDynamicsContext(options);
        context.Database.EnsureCreated();

        // Seed required entities if not already present
        if (!context.Operations.Any())
        {
            context.Operations.AddRange(
                new Operation { Id = 1, Name = "CheckOut" },
                new Operation { Id = 2, Name = "CheckIn" }
            );
        }

        if (!context.Crews.Any())
        {
            context.Crews.Add(new Crew { Id = 1, Name = "Janet" });
        }

        if (!context.Equipment.Any())
        {
            context.Equipment.Add(new Equipment { Id = 1, Name = "Toolkit" });
        }

        context.SaveChanges();
        return context;
    }

    [Fact]
    public async Task GetAll_ShouldReturnTransactionsWithIncludes()
    {
        // Arrange
        var context = GetInMemoryContext();

        context.Transactions.Add(new Transaction
        {
            CrewId = 1,
            EquipmentId = 1,
            OperationId = 1,
            Date = DateTime.UtcNow
        });

        context.SaveChanges();

        var controller = new TransactionController(context);

        // Act
        var result = await controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();

        var data = (result.Result as OkObjectResult)!.Value as List<Transaction>;

        data.Should().ContainSingle();

        var txn = data![0];

        txn.Crew.Id.Should().Be(1);
        txn.Crew.Name.Should().NotBeNullOrEmpty();

        txn.Equipment.Id.Should().Be(1);
        txn.Equipment.Name.Should().NotBeNullOrEmpty();

        txn.Operation.Id.Should().Be(1);
        txn.Operation.Name.Should().Be("CheckOut");
    }

    [Fact]
    public async Task Create_ShouldAddTransaction()
    {
        // Arrange
        var context = GetInMemoryContext();
        var controller = new TransactionController(context);

        var newTransaction = new Transaction
        {
            CrewId = 1,
            EquipmentId = 1,
            OperationId = 1,
            Date = DateTime.UtcNow
        };

        // Act
        var result = await controller.Create(newTransaction);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        context.Transactions.Should().ContainSingle();
    }
}
