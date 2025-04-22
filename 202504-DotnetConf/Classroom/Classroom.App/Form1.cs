using System.Globalization;

using Classroom.App.Client.Models;

using Microsoft.Extensions.DependencyInjection;

using attList = System.Collections.Generic.List<Classroom.App.Client.Models.WeeklyAttendanceRowModel>;

namespace Classroom.App;

public partial class Form1 : Form
{
    private readonly IClassroomClient _client;
    private List<ClassModel> _classes = [];
    private attList _rows = [];
    private readonly DateOnly _weekStart = GetCurrentWeekStart();

    public Form1()
    {
        InitializeComponent();
        _client = Program.Services.GetRequiredService<IClassroomClient>();
        Load += Form1_Load;
        saveButton.Click += SaveButton_Click;
    }

    private async void Form1_Load(object? sender, EventArgs e)
    {
        _classes = await _client.GetAvailableClasses();

        classPicker.DataSource = _classes;
        classPicker.DisplayMember = "Name";
        classPicker.ValueMember = "Id";

        classPicker.SelectedIndexChanged += async (_, _) =>
        {
            if (classPicker.SelectedItem is ClassModel selected)
            {
                await LoadAttendance(selected.Id);
            }
        };

        if (_classes.Count > 0)
        {
            classPicker.SelectedIndex = 0;
        }

        attendanceGrid.ReadOnly = false;
        attendanceGrid.EditMode = DataGridViewEditMode.EditOnEnter;
        attendanceGrid.CellContentClick += (_, e) =>
        {
            if (attendanceGrid.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn &&
                e.RowIndex >= 0)
            {
                var cell = attendanceGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                var value = cell.Value as bool? ?? false;
                cell.Value = !value;

                // These lines ensure proper update
                attendanceGrid.EndEdit();
                attendanceGrid.RefreshEdit();

                // Update the data model to match the UI state
                if (e.RowIndex < _rows.Count && e.ColumnIndex > 0 && e.ColumnIndex <= 5)
                {
                    var weekDates = GetSchoolWeek(_weekStart);
                    var date = weekDates[e.ColumnIndex - 1]; // -1 because first column is Name
                    var student = _rows[e.RowIndex];

                    if (!student.WeekAttendance.TryGetValue(date, out var attendance))
                    {
                        student.WeekAttendance[date] = new AttendanceCellModel { Present = !value };
                    }
                    else if (attendance != null)
                    {
                        attendance.Present = !value;
                    }
                }
            }
        };
    }

    private async void SaveButton_Click(object? sender, EventArgs e)
    {
        var weekDates = GetSchoolWeek(_weekStart);

        for (int rowIndex = 0; rowIndex < _rows.Count; rowIndex++)
        {
            var row = _rows[rowIndex];

            for (int colIndex = 0; colIndex < weekDates.Length; colIndex++)
            {
                var date = weekDates[colIndex];
                var cellValue = attendanceGrid.Rows[rowIndex].Cells[colIndex + 1].Value;

                row.WeekAttendance[date] = new AttendanceCellModel
                {
                    AttendanceId = row.WeekAttendance.GetValueOrDefault(date)?.AttendanceId,
                    Present = cellValue as bool? ?? false
                };
            }
        }

        if (classPicker.SelectedItem is ClassModel selected)
        {
            await _client.SaveWeeklyAttendance(selected.Id, _weekStart, _rows);
            // MessageBox.Show("Attendance saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private async Task LoadAttendance(int classId)
    {
        _rows = await _client.LoadWeeklyAttendance(classId, _weekStart);
        attendanceGrid.Columns.Clear();
        attendanceGrid.Rows.Clear();

        // Setup columns
        attendanceGrid.Columns.Add("Name", "Student");
        attendanceGrid.Columns[0].ReadOnly = true;

        var weekDates = GetSchoolWeek(_weekStart);

        foreach (var day in weekDates)
        {
            var checkboxColumn = new DataGridViewCheckBoxColumn
            {
                Name = day.ToString("ddd", CultureInfo.InvariantCulture),
                HeaderText = day.ToString("ddd"),
                TrueValue = true,
                FalseValue = false,
                ThreeState = false,
                ReadOnly = false,
                Width = 50
            };
            attendanceGrid.Columns.Add(checkboxColumn);
        }

        // Populate rows
        foreach (var row in _rows)
        {
            var cells = new List<object> { row.Name };
            foreach (var date in weekDates)
            {
                var cell = row.WeekAttendance.GetValueOrDefault(date);
                cells.Add(cell?.Present ?? false);
            }
            attendanceGrid.Rows.Add(cells.ToArray());
        }
    }

    private static DateOnly GetCurrentWeekStart()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var dow = (int)today.DayOfWeek;
        return today.AddDays(-(dow == 0 ? 6 : dow - 1));
    }

    private static DateOnly[] GetSchoolWeek(DateOnly start)
    {
        return
        [
            start,
            start.AddDays(1),
            start.AddDays(2),
            start.AddDays(3),
            start.AddDays(4)
        ];
    }
}
