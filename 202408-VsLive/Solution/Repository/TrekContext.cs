using Microsoft.EntityFrameworkCore;

namespace Api.Repository;

public class TrekContext : DbContext
{
    private const string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Database=HR;Integrated Security=True;";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(connectionString);
    }

    public DbSet<Department> Departments { get; set; }

    public DbSet<Employee> Employees { get; set; }

    public Employee ReorganizeEmployee(int employeeId, int departmentId)
    {
        Database.ExecuteSqlRaw(
            "EXEC [dbo].[ReorgEmployee] @EmployeeId = {0}, @DepartmentId = {1}",
            employeeId, departmentId);

        return Employees.SingleOrDefault(e => e.Id == employeeId)
            ?? throw new InvalidOperationException($"Employee with ID {employeeId} not found.");
    }
}