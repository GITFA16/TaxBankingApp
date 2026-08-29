using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TaxBankingApi.Authentication;
using TaxBankingApi.Data;

var builder = WebApplication.CreateBuilder(args);


// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);


// Controllers
builder.Services.AddControllers();


// Basic Authentication
builder.Services
    .AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(
        "BasicAuthentication",
        null
    );


// Require authentication for all controllers
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});


// Swagger
builder.Services.AddOpenApiDocument(options =>
{
    options.DocumentName = "v1";
    options.Title = "Tax Banking API";
    options.Version = "v1";
});


// CORS for Vue frontend
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


// Allow frontend
app.UseCors("AllowFrontend");


// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(options =>
        options.Path = "/swagger/{documentName}/swagger.json");

    app.UseSwaggerUi(options =>
    {
        options.Path = "/swagger";
        options.DocumentPath =
            "/swagger/{documentName}/swagger.json";
    });
}


// HTTPS redirect
app.UseHttpsRedirection();


// Authentication must be before Authorization
app.UseAuthentication();

app.UseAuthorization();


// API Controllers
app.MapControllers();

app.Run();