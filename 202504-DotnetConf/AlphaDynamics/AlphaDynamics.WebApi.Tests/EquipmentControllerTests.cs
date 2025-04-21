using AlphaDynamics.Poco;
using AlphaDynamics.SqlRepository;
using AlphaDynamics.WebApi.Controllers;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlphaDynamics.WebApi.Tests;

public class EquipmentControllerTests
{
    private AlphaDynamicsContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AlphaDynamicsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AlphaDynamicsContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllEquipment()
    {
        // Arrange
        var context = GetInMemoryContext();
        context.Equipment.Add(new Equipment { Name = "Test Tool" });
        context.SaveChanges();
        var controller = new EquipmentController(context);

        // Act
        var result = await controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var data = (result.Result as OkObjectResult)!.Value as List<Equipment>;
        data.Should().ContainSingle(e => e.Name == "Test Tool");
    }

    [Fact]
    public async Task Create_ShouldAddNewEquipment()
    {
        // Arrange
        var context = GetInMemoryContext();
        var controller = new EquipmentController(context);
        var newEquipment = new Equipment { Name = "Oxygen Tank" };

        // Act
        var result = await controller.Create(newEquipment);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        context.Equipment.Should().ContainSingle(e => e.Name == "Oxygen Tank");
    }
}
