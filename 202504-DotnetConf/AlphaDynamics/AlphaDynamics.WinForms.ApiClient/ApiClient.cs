using System.Net.Http.Json;

using AlphaDynamics.Poco;

namespace AlphaDynamics.WinForms;

public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http)
    {
        _http = http;
        _http.BaseAddress = new Uri("http://localhost:1234");
    }

    public async Task<List<Crew>> GetCrewAsync()
    {
        var response = await _http.GetAsync("/api/crew");
        if (!response.IsSuccessStatusCode)
            return [];

        return await response.Content.ReadFromJsonAsync<List<Crew>>() ?? [];
    }


    public async Task<List<Equipment>> GetEquipmentAsync() =>
        await _http.GetFromJsonAsync<List<Equipment>>("/api/equipment") ?? [];

    public async Task<List<Transaction>> GetTransactionsAsync() =>
        await _http.GetFromJsonAsync<List<Transaction>>("/api/transaction") ?? [];

    public async Task PostTransactionAsync(Transaction transaction)
    {
        await _http.PostAsJsonAsync("/api/transaction", transaction);
    }
}
