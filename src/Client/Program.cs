using Json;

using System.Diagnostics;
using System.Net.Http.Json;

var baseUrl = Environment.GetEnvironmentVariable("DabUrl");
var uri = $"{baseUrl}/api/User/Id/1?$select=FirstName";

var result = await new HttpClient()
    .GetFromJsonAsync<Root<User>>(uri)
    ?? throw new Exception("Failed to deserialize.");
var record = result.Value.Single();

Debugger.Break();

namespace Json
{
    public class Root<T>
    {
        public List<T> Value { get; set; } = [];
    }

    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string LAstName { get; set; } = default!;
    }
}