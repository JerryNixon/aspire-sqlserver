using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Repository;

[Table("Employee")]
public record Employee(int Id, string First, string Last, int DepartmentId)
{
    public Department GetDepartment()
    {
        using var db = new HrContext();
        return db.Departments
            .Single(x => x.Id == DepartmentId);
    }
}