using System.Configuration;
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

// Servicios
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddTransient<IRestProvider, RestProvider>();
builder.Services.AddScoped<RestProvider>();


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRepositoryUser, RepositoryUser>();

builder.Services.AddScoped<EmailProvider>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IRepositoryCard, RepositoryCard>();
builder.Services.AddScoped<IBusinessCard, BusinessCard>();

//builder.Services.AddScoped<CardService>();
builder.Services.AddScoped<ICardService, CardService>();


builder.Services.AddScoped<IRepositoryList, RepositoryList>();
builder.Services.AddScoped<IBusinessList, BusinessList>();
builder.Services.AddScoped<ListService>();

builder.Services.AddScoped<IRepositoryLabel, RepositoryLabel>();
builder.Services.AddScoped<IBusinessLabel, BusinessLabel>();
builder.Services.AddScoped<LabelService>();

builder.Services.AddScoped<IRepositoryBoard, RepositoryBoard>();
builder.Services.AddScoped<IBusinessBoard, BusinessBoard>();
builder.Services.AddScoped<BoardService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRepositoryBoardMember, RepositoryBoardMember>();
builder.Services.AddScoped<IBusinessBoardMember, BusinessBoardMember>();
builder.Services.AddScoped<BoardMemberService>();

builder.Services.AddScoped<IRepositoryComment, RepositoryComment>();
builder.Services.AddScoped<IBusinessComment, BusinessComment>();
builder.Services.AddScoped<CommentService>();

builder.Services.AddScoped<IRepositoryNotification, RepositoryNotification>();
builder.Services.AddScoped<IBusinessNotification, BusinessNotification>();
builder.Services.AddScoped<NotificationService>();

builder.Services.AddDbContext<TrelloDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ListService>();

// Repositories
builder.Services.AddScoped<IRepositoryUser, RepositoryUser>();

// Business
builder.Services.AddScoped<IBusinessUser, BusinessUser>();

// Services
builder.Services.AddScoped<IUserService, UserService>();


var app = builder.Build();

// Middleware pipeline
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
