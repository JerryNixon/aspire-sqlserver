using System.Net.Http.Json;

var readWriteUrl = "http://localhost:5000/api/InfoReadWrite";
var readOnlyUrl = "http://localhost:5000/api/InfoReadOnly";

var http = new HttpClient();

var rw = await http.GetFromJsonAsync<Rootobject>(readWriteUrl);

Console.WriteLine(rw.value.Single().ServerInfo);

var ro = await http.GetFromJsonAsync<Rootobject>(readOnlyUrl);

Console.WriteLine(ro.value.Single().ServerInfo);

Console.ReadLine();

public class Rootobject
{
    public Value[] value { get; set; }
}

public class Value
{
    public string ServerInfo { get; set; }
}

/*

var result = await GetScalarValueAsync();

Console.WriteLine(result);
Console.ReadLine();

async Task<string?> GetScalarValueAsync()
{
    var connString = "Server=tcp:dotnet-conf-sql.database.windows.net,1433;Initial Catalog=dotnet-conf-db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";";
    connString += ";ApplicationIntent=ReadOnly;";
    using var connection = new SqlConnection(connString);
    await connection.OpenAsync();
    using var command = connection.CreateCommand();
    command.CommandText = "select ServerInfo from Info;";
    return (string?)await command.ExecuteScalarAsync();
}

*/