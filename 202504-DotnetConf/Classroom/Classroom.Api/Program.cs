using Classroom.Api.Repository;
using Classroom.Poco;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ClassroomRepository>();

var app = builder.Build();

app.MapGet("/students", async (int? classId, ClassroomRepository repo) =>
{
    var all = await repo.GetStudents();

    if (classId.HasValue)
    {
        return Results.Ok(all.Where(s => s.ClassId == classId.Value).ToList());
    }

    return Results.Ok(all);
});

app.MapGet("/classes", async (ClassroomRepository repo) =>
{
    var result = await repo.GetClasses();
    return Results.Ok(result);
});

app.MapGet("/attendance", async (int? classId, DateOnly? startDate, DateOnly? endDate, ClassroomRepository repo) =>
{
    var all = await repo.GetAttendance();

    if (classId.HasValue)
    {
        all = all.Where(a => a.ClassId == classId.Value).ToList();
    }

    if (startDate.HasValue)
    {
        all = all.Where(a => a.Date >= startDate.Value).ToList();
    }

    if (endDate.HasValue)
    {
        all = all.Where(a => a.Date <= endDate.Value).ToList();
    }

    return Results.Ok(all);
});

app.MapPost("/attendance", async (AttendancePoco record, ClassroomRepository repo) =>
{
    var result = await repo.AddAttendance(record);
    return Results.Created($"/attendance/{result.Id}", result);
});

app.MapPut("/attendance/{attendanceId}", async (int attendanceId, AttendancePoco input, ClassroomRepository repo) =>
    await repo.UpdateAttendance(attendanceId, input) ? Results.NoContent() : Results.NotFound());

app.MapDelete("/attendance/{attendanceId}", async (int attendanceId, ClassroomRepository repo) =>
    await repo.DeleteAttendance(attendanceId) ? Results.NoContent() : Results.NotFound());

app.Run();