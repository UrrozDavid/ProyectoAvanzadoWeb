using Microsoft.EntityFrameworkCore;
using TBA.Data.Models;
using TBA.Repositories;
using TBA.Business;
using TBA.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Configuración de DbContext
// --------------------
builder.Services.AddDbContext<TrelloDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TrelloDB")));

// --------------------
// Servicios de negocio
// --------------------
builder.Services.AddScoped<IBusinessCard, BusinessCard>();
builder.Services.AddScoped<IBusinessComment, BusinessComment>();
builder.Services.AddScoped<IBusinessNotification, BusinessNotification>();
builder.Services.AddScoped<IRepositoryCard, RepositoryCard>();
builder.Services.AddScoped<IRepositoryComment, RepositoryComment>();
builder.Services.AddScoped<IRepositoryNotification, RepositoryNotification>();

//builder.Services.AddScoped<IBusinessChecklistItem, BusinessChecklistItem>();

// --------------------
// CORS para desarrollo local (MVC en localhost:7010)
// --------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhostMvc", policy =>
    {
        policy.WithOrigins("https://localhost:7010") // Cambia si tu MVC usa otro puerto
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// --------------------
// Controllers y Swagger
// --------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------
// Construcción de la app
// --------------------
var app = builder.Build();

// --------------------
// Middleware
// --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TBA API V1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz para fácil acceso
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowLocalhostMvc");
app.UseAuthorization();

// --------------------
// Mapear controllers
// --------------------
app.MapControllers();

// --------------------
// Ejecutar la app
// --------------------
app.Run();
