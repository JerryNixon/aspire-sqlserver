using System.Net.Http.Json;
using System.Text.Json;

using Classroom.App.Client.Models;
using Classroom.Poco;

using Microsoft.Extensions.Logging;

using attList = System.Collections.Generic.List<Classroom.App.Client.Models.WeeklyAttendanceRowModel>;

public class ClassroomApiClient : IClassroomClient
{
    private readonly HttpClient _http;
    private readonly ILogger<ClassroomApiClient> _logger;

    public ClassroomApiClient(string baseUrl, ILogger<ClassroomApiClient> logger)
    {
        _http = new HttpClient { BaseAddress = new Uri(baseUrl.TrimEnd('/')) };
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<ClassModel>> GetAvailableClasses()
    {
        var results = await GetAsync<ClassPoco>("/classes");
        return results.Select(c => new ClassModel
        {
            Id = c.Id,
            Name = c.Name
        }).ToList();
    }

    public async Task<attList> LoadWeeklyAttendance(int classId, DateOnly anyDateInWeek)
    {
        _logger.LogInformation("Loading weekly attendance for class {ClassId} week of {WeekStart}", classId, anyDateInWeek);

        var weekDates = GetSchoolWeek(anyDateInWeek);
        var startDate = weekDates.First();
        var endDate = weekDates.Last();

        var students = await GetAsync<StudentPoco>($"/students?classId={classId}");
        var attendance = await GetAsync<AttendancePoco>($"/attendance?classId={classId}&startDate={startDate}&endDate={endDate}");

        var result = new attList();

        foreach (var student in students)
        {
            var row = new WeeklyAttendanceRowModel
            {
                StudentId = student.Id,
                Name = student.Name
            };

            foreach (var date in weekDates)
            {
                var match = attendance.FirstOrDefault(a =>
                    a.StudentId == student.Id &&
                    a.Date == date);

                row.WeekAttendance[date] = match == null
                    ? null
                    : new AttendanceCellModel
                    {
                        AttendanceId = match.Id,
                        Present = match.Present
                    };
            }

            result.Add(row);
        }

        return result;
    }

    public async Task SaveWeeklyAttendance(int classId, DateOnly weekStartDate, attList rows)
    {
        _logger.LogInformation("Saving weekly attendance for class {ClassId} week of {WeekStart}", classId, weekStartDate);

        var tasks = new List<Task>();

        foreach (var row in rows)
        {
            foreach (var kvp in row.WeekAttendance)
            {
                var date = kvp.Key;
                var cell = kvp.Value;

                if (cell == null) continue;

                var record = new AttendancePoco
                {
                    Id = cell.AttendanceId ?? 0,
                    ClassId = classId,
                    StudentId = row.StudentId,
                    Date = date,
                    Present = cell.Present
                };

                var path = cell.AttendanceId == null
                    ? "/attendance"
                    : $"/attendance/{record.Id}";

                var method = cell.AttendanceId == null ? "POST" : "PUT";

                _logger.LogDebug("{Method} attendance for StudentId {StudentId} on {Date}", method, record.StudentId, record.Date);

                tasks.Add(cell.AttendanceId == null
                    ? _http.PostAsJsonAsync(path, record)
                    : _http.PutAsJsonAsync(path, record));
            }
        }

        await Task.WhenAll(tasks);
    }

    private static readonly JsonSerializerOptions _camelCaseOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private async Task<List<T>> GetAsync<T>(string path)
    {
        for (int i = 0; i < 3; i++)
        {
            try
            {
                var response = await _http.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<List<T>>(json, _camelCaseOptions);
                    return result ?? [];
                }
            }
            catch (HttpRequestException)
            {
                if (i == 2) throw;
                await Task.Delay(1000 * (int)Math.Pow(2, i) + Random.Shared.Next(0, 1000));
            }
        }

        return [];
    }

    private static DateOnly[] GetSchoolWeek(DateOnly anyDate)
    {
        var dow = (int)anyDate.DayOfWeek;
        var monday = anyDate.AddDays(-(dow == 0 ? 6 : dow - 1));

        return [
            monday,
            monday.AddDays(1),
            monday.AddDays(2),
            monday.AddDays(3),
            monday.AddDays(4)
        ];
    }
}
