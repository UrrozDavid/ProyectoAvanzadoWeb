
using APW.Architecture;
using TBA.Architecture.Providers;
using TBA.Core.Settings;
using TBA.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IRestProvider, RestProvider>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<EmailProvider>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));


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
