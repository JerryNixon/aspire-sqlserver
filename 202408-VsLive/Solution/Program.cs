using Api.Repository;

using HotChocolate.Subscriptions;

var builder = WebApplication
    .CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSubscriptionType<Subscription>()
    .AddInMemorySubscriptions();

var app = builder.Build();
app.UseWebSockets();
app.MapGraphQL();
app.Run();

public class Query
{
    public Employee GetEmployee(int id)
    {
        using var db = new HrContext();
        return db.Employees
            .Where(x => x.Id == id)
            .Single();
    }

    public IEnumerable<Department> GetDepartments(int? id = null, string? name = null)
    {
        using var db = new HrContext();
        return db.Departments
            .Where(x => id == null || x.Id == id)
            .Where(x => name == null || x.Name.Contains(name));
    }
}

public partial class Mutation
{
    [GraphQLName("ReorgEmployee")]
    [GraphQLDescription("Moves an employee to new Department.")]
    public async Task<Employee> ReorgEmployeeAsync([Service] ITopicEventSender eventSender, int employeeId, int departmentId)
    {
        using var db = new HrContext();
        var employee = db.ReorganizeEmployee(employeeId, departmentId);
        await eventSender.SendAsync(nameof(Subscription.OnReorg), employee);
        return employee;
    }
}

public partial class Subscription
{
    [Subscribe]
    [Topic("Jerry")]
    [GraphQLDescription("After an employee has been reorg'd.")]
    public Employee OnReorg([EventMessage] Employee employee) => employee;
}