using Microsoft.EntityFrameworkCore; //EF c# to database mapping
using TaxBankingApi.Data;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite( //AppDbContext akan menggunakan SQLite.
        builder.Configuration.GetConnectionString("DefaultConnection")
        //get connection string named DefaultConnection from appsettings.json.
    )
);
//AppDbContext to ASP.NET Dependency Injection.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
