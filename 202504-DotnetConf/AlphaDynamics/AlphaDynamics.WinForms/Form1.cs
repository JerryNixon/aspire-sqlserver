namespace AlphaDynamics.WinForms;

public partial class Form1 : Form
{
    private readonly ApiClient _api;

    public Form1(ApiClient api)
    {
        InitializeComponent();
        _api = api;
    }

    private async void Form1_Load(object sender, EventArgs e)
    {
        var crew = await _api.GetCrewAsync();
        crewListBox.DataSource = crew;
        crewListBox.DisplayMember = "Name";
    }
}
