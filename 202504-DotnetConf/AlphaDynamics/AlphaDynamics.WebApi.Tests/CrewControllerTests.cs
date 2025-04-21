using AlphaDynamics.Poco;
using AlphaDynamics.SqlRepository;
using AlphaDynamics.WebApi.Controllers;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlphaDynamics.WebApi.Tests;

public class CrewControllerTests
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
    public async Task GetAll_ShouldReturnAllCrew()
    {
        // Arrange
        var context = GetInMemoryContext();
        context.Crews.Add(new Crew { Name = "Test Crew" });
        context.SaveChanges();
        var controller = new CrewController(context);

        // Act
        var result = await controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var data = (result.Result as OkObjectResult)!.Value as List<Crew>;
        data.Should().ContainSingle(c => c.Name == "Test Crew");
    }

    [Fact]
    public async Task Create_ShouldAddNewCrew()
    {
        // Arrange
        var context = GetInMemoryContext();
        var controller = new CrewController(context);
        var newCrew = new Crew { Name = "New Member" };

        // Act
        var result = await controller.Create(newCrew);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        context.Crews.Should().ContainSingle(c => c.Name == "New Member");
    }
}
