using Classroom.App.Client.Models;

using attList = System.Collections.Generic.List<Classroom.App.Client.Models.WeeklyAttendanceRowModel>;

public interface IClassroomClient
{
    Task<List<ClassModel>> GetAvailableClasses();
    Task<attList> LoadWeeklyAttendance(int classId, DateOnly anyDateInWeek);
    Task SaveWeeklyAttendance(int classId, DateOnly weekStartDate, attList rows);
}
