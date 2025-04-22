namespace Classroom.Poco;

public class StudentPoco
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int ClassId { get; set; }
}

public class ClassPoco
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}

public class AttendancePoco
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public int StudentId { get; set; }
    public DateOnly Date { get; set; }
    public bool Present { get; set; }
}
