using Microsoft.EntityFrameworkCore; //EF c# to database mapping
using Microsoft.Extensions.Options;
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

// builder.Services.AddOpenApi();

// Add code block to enable Swagger UI
builder.Services.AddOpenApiDocument(options =>
{
    options.DocumentName = "v1";
    options.Title = "Tax Banking API";
    options.Version = "v1";
}
);
//connecting with frontend localhost 5173
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi(); 
    //add swagger
    app.UseOpenApi(options =>
        options.Path = "/swagger/{documentName}/swagger.json");

    app.UseSwaggerUi(options =>
    {
        options.Path = "/swagger";
        options.DocumentPath = "/swagger/{documentName}/swagger.json";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
