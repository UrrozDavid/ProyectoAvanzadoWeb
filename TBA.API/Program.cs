using Microsoft.EntityFrameworkCore;
using TBA.Business;
using TBA.Data.Models;
using TBA.Repositories;
using TBA.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepositoryUser, RepositoryUser>();
builder.Services.AddScoped<IBusinessUser, BusinessUser>();
builder.Services.AddScoped<IRepositoryNotification, RepositoryNotification>();
builder.Services.AddScoped<IBusinessNotification, BusinessNotification>();
builder.Services.AddScoped<IRepositoryList, RepositoryList>();
builder.Services.AddScoped<IBusinessList, BusinessList>();
builder.Services.AddScoped<IRepositoryLabel, RepositoryLabel>();
builder.Services.AddScoped<IBusinessLabel, BusinessLabel>();
builder.Services.AddScoped<IRepositoryComment, RepositoryComment>();
builder.Services.AddScoped<IBusinessComment, BusinessComment>();
builder.Services.AddScoped<IRepositoryCard, RepositoryCard>();
builder.Services.AddScoped<IBusinessCard, BusinessCard>();
builder.Services.AddScoped<IRepositoryBoardMember, RepositoryBoardMember>();
builder.Services.AddScoped<IBusinessBoardMember, BusinessBoardMember>();
builder.Services.AddScoped<IRepositoryBoard, RepositoryBoard>();
builder.Services.AddScoped<IBusinessBoard, BusinessBoard>();
builder.Services.AddScoped<IRepositoryAttachment, RepositoryAttachment>();
builder.Services.AddScoped<IBusinessAttachment, BusinessAttachment>();
builder.Services.AddDbContext<TrelloDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TrelloDbContext")));
builder.Services.AddScoped<IRepositoryCard, RepositoryCard>();
builder.Services.AddScoped<IBusinessCard, BusinessCard>();
//builder.Services.AddScoped<ICardService, CardService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
