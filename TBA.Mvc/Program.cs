
using APW.Architecture;
using TBA.Architecture.Providers;
using TBA.Business;
using TBA.Core.Settings;
using TBA.Models.Entities;
using TBA.Repositories;
using TBA.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IRestProvider, RestProvider>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<EmailProvider>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));


//builder.Services.AddScoped<IRepositoryCard, RepositoryCard>();
//builder.Services.AddScoped<IBusinessCard, BusinessCard>();
builder.Services.AddScoped<ICardService, CardService>();


// Registro repositorio
builder.Services.AddScoped<IRepositoryList, RepositoryList>();

// Registro negocio
builder.Services.AddScoped<IBusinessList, BusinessList>();

// Registro servicios
builder.Services.AddScoped<ListService>();

// Registro repositorio
builder.Services.AddScoped<IRepositoryLabel, RepositoryLabel>();

// Registro negocio
builder.Services.AddScoped<IBusinessLabel, BusinessLabel>();

// Registro servicios
builder.Services.AddScoped<LabelService>();

// Registro repositorios
builder.Services.AddScoped<IRepositoryBoard, RepositoryBoard>();

// Registro negocio
builder.Services.AddScoped<IBusinessBoard, BusinessBoard>();

// Registro servicios
builder.Services.AddScoped<BoardService>();


// Repositorio
builder.Services.AddScoped<IRepositoryBoardMember, RepositoryBoardMember>();

// Negocio
builder.Services.AddScoped<IBusinessBoardMember, BusinessBoardMember>();

// Servicio
builder.Services.AddScoped<BoardMemberService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
