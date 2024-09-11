using Microsoft.EntityFrameworkCore;
using FleetManagementSystem.Data;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMvc();
//builder.Services.AddScoped<FleetManagementDbContext, FleetManagementDbContext>();
// Add SQLite DB context for FleetManagementDbContext
builder.Services.AddDbContext<FleetManagementDbContext>(options =>
    options.UseSqlite("Data Source=FleetManagementSystem.db"));

// Configure FleetManagementDbContext with SQLite
//builder.Services.AddDbContext<FleetManagementDbContext>(options =>
//    options.UseSqlite(builder.Configuration.GetConnectionString("FleetManagementConnection")));

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
    pattern: "{controller=Home}/{action=Index}/{id?}/{searchQuery?}");

app.Run();

