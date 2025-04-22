using Classroom.Poco;

using Microsoft.EntityFrameworkCore;

namespace Classroom.Api.Repository;

public class ClassroomRepository
{
    private readonly ClassroomContext _db;

    public ClassroomRepository() => _db = new();

    public Task<List<StudentPoco>> GetStudents() => _db.Students.ToListAsync();
    public Task<List<ClassPoco>> GetClasses() => _db.Classes.ToListAsync();
    public Task<List<AttendancePoco>> GetAttendance() => _db.Attendance.ToListAsync();

    public async Task<AttendancePoco> AddAttendance(AttendancePoco record)
    {
        _db.Attendance.Add(record);
        await _db.SaveChangesAsync();
        return record;
    }

    public async Task<bool> UpdateAttendance(int attendanceId, AttendancePoco input)
    {
        var record = await _db.Attendance.FindAsync(attendanceId);
        if (record is null) return false;

        record.ClassId = input.ClassId;
        record.StudentId = input.StudentId;
        record.Date = input.Date;
        record.Present = input.Present;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAttendance(int attendanceId)
    {
        var record = await _db.Attendance.FindAsync(attendanceId);
        if (record is null) return false;

        _db.Attendance.Remove(record);
        await _db.SaveChangesAsync();
        return true;
    }
}