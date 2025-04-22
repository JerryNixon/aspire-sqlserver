using System.Text.Json.Serialization;

namespace Classroom.App.Client.Models;

public class ClassModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class StudentModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int ClassId { get; set; }
}

public class AttendanceModel
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public int StudentId { get; set; }
    public DateOnly Date { get; set; }
    public bool Present { get; set; }
}

public class WeeklyAttendanceRowModel
{
    public int StudentId { get; set; }
    public string Name { get; set; } = "";
    public Dictionary<DateOnly, AttendanceCellModel?> WeekAttendance { get; set; } = new();
}

public class AttendanceCellModel
{
    public int? AttendanceId { get; set; }
    public bool Present { get; set; }
}

public class DabRoot<T>
{
    [JsonPropertyName("value")]
    public T[]? Value { get; set; }
}