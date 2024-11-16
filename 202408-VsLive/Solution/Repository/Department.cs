using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Repository;

[Table("Department")]
public record Department(int Id, string Name)
{
    public IEnumerable<Employee> GetEmployees()
    {
        using var db = new HrContext();
        return db.Employees
            .Where(x => x.DepartmentId == Id);
    }
}
