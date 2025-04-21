using AlphaDynamics.SqlRepository;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var connStr = builder.Configuration.GetConnectionString("Default")!;
builder.Services.AddAlphaDynamicsRepository(connStr);

var app = builder.Build();
app.MapControllers();
app.Run();
