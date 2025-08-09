using APW.Architecture;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TBA.Architecture.Providers;
using TBA.Business;
using TBA.Core.Settings;
using TBA.Data.Models;
using TBA.Models.Entities;
using TBA.Repositories;
using TBA.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Autenticación por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// Infraestructura
builder.Services.AddTransient<IRestProvider, RestProvider>();
builder.Services.AddScoped<RestProvider>();
builder.Services.AddScoped<EmailProvider>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// DbContext
builder.Services.AddDbContext<TrelloDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IRepositoryUser, RepositoryUser>();
builder.Services.AddScoped<IRepositoryCard, RepositoryCard>();
builder.Services.AddScoped<IRepositoryList, RepositoryList>();
builder.Services.AddScoped<IRepositoryLabel, RepositoryLabel>();
builder.Services.AddScoped<IRepositoryBoard, RepositoryBoard>();
builder.Services.AddScoped<IRepositoryBoardMember, RepositoryBoardMember>();
builder.Services.AddScoped<IRepositoryComment, RepositoryComment>();
builder.Services.AddScoped<IRepositoryChecklistItem, RepositoryChecklistItem>();
builder.Services.AddScoped<IRepositoryNotification, RepositoryNotification>();

// Business
builder.Services.AddScoped<IBusinessUser, BusinessUser>();
builder.Services.AddScoped<IBusinessCard, BusinessCard>();
builder.Services.AddScoped<IBusinessList, BusinessList>();
builder.Services.AddScoped<IBusinessLabel, BusinessLabel>();
builder.Services.AddScoped<IBusinessBoard, BusinessBoard>();
builder.Services.AddScoped<IBusinessBoardMember, BusinessBoardMember>();
builder.Services.AddScoped<IBusinessComment, BusinessComment>();
builder.Services.AddScoped<IBusinessNotification, BusinessNotification>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IChecklistService, ChecklistService>();
//builder.Services.AddScoped<IListService, ListService>(); // Faltaba interfaz
builder.Services.AddScoped<ILabelService, LabelService>();
//builder.Services.AddScoped<IBoardService, BoardService>(); // Faltaba interfaz
builder.Services.AddScoped<IBoardMemberService, BoardMemberService>();
//builder.Services.AddScoped<ICommentService, CommentService>(); // Faltaba interfaz
//builder.Services.AddScoped<INotificationService, NotificationService>(); // Faltaba interfaz

builder.Services.AddScoped<IRepositoryCard, RepositoryCard>();

builder.Services.AddScoped<IRepositoryBase<Label>, RepositoryLabel>();
builder.Services.AddScoped<IBusinessChecklistItem, BusinessChecklistItem>();
builder.Services.AddScoped<BoardService>();





var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
