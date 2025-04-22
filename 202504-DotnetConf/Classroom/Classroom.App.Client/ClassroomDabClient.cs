using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

using Classroom.App.Client.Models;

using Microsoft.Extensions.Logging;

using attList = System.Collections.Generic.List<Classroom.App.Client.Models.WeeklyAttendanceRowModel>;

public class ClassroomDabClient : IClassroomClient
{
    private readonly HttpClient _http;

    public ClassroomDabClient(string baseUrl, ILogger<ClassroomDabClient> logger)
    {
        _http = new HttpClient { BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/api/") };
        _logger = logger;
    }

    public async Task<List<ClassModel>> GetAvailableClasses()
    {
        return await GetAsync<ClassModel>("Classes");
    }

    public async Task<attList> LoadWeeklyAttendance(int classId, DateOnly anyDateInWeek)
    {
        var weekDates = GetSchoolWeek(anyDateInWeek);

        var startIso = $"Date ge {weekDates.First():yyyy-MM-dd}T00:00:00Z";
        var endIso = $"Date le {weekDates.Last():yyyy-MM-dd}T00:00:00Z";
        var dateRangeFilter = $"ClassId eq {classId} and {startIso} and {endIso}";
        var encodedRangeFilter = HttpUtility.UrlEncode(dateRangeFilter);

        var students = await GetAsync<StudentModel>($"Students?$filter=ClassId eq {classId}");
        var attendance = await GetAsync<AttendanceModel>($"Attendance?$filter={encodedRangeFilter}");

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
                    a.StudentId == student.Id && a.Date == date);

                if (match == null)
                {
                    row.WeekAttendance[date] = null;
                }
                else
                {
                    row.WeekAttendance[date] = new AttendanceCellModel
                    {
                        AttendanceId = match.Id,
                        Present = match.Present
                    };
                }
            }

            result.Add(row);
        }

        return result;
    }

    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNamingPolicy = null, // required for PascalCase field names like "ClassId"
        Converters = { new DateOnlyJsonConverter() },
    };

    private readonly ILogger<ClassroomDabClient> _logger;

    public async Task SaveWeeklyAttendance(int classId, DateOnly weekStartDate, attList rows)
    {
        _logger.LogInformation("Starting SaveWeeklyAttendance for ClassId {ClassId}, WeekStartDate {WeekStartDate}", classId, weekStartDate);

        foreach (var row in rows)
        {
            foreach (var kvp in row.WeekAttendance)
            {
                var date = kvp.Key;
                var cell = kvp.Value;

                if (cell == null)
                    continue;

                var record = new
                {
                    ClassId = classId,
                    StudentId = row.StudentId,
                    Date = date,
                    Present = cell.Present
                };

                var bodyJson = JsonSerializer.Serialize(record, _jsonOpts);

                HttpResponseMessage response;

                if (cell.AttendanceId is null)
                {
                    _logger.LogInformation("{Method} Body {Body}", "POST", bodyJson);
                    response = await _http.PostAsJsonAsync("Attendance", record, _jsonOpts);
                }
                else
                {
                    _logger.LogInformation("{Method} Body {Body}", "PUT", bodyJson);
                    response = await _http.PutAsJsonAsync($"Attendance/Id/{cell.AttendanceId}", record, _jsonOpts);
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to save attendance for StudentId {StudentId} on {Date}: {Error}", row.StudentId, date, error);
                    throw new HttpRequestException($"Failed to save attendance: {error}");
                }
            }
        }

        _logger.LogInformation("Completed SaveWeeklyAttendance");
    }

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
                    var models = JsonSerializer.Deserialize<DabRoot<T>>(json, _jsonOpts);
                    return models?.Value?.ToList() ?? [];
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

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateOnly.Parse(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
    }
}