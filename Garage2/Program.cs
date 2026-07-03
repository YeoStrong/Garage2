using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Garage2Context") ?? throw new InvalidOperationException("Connection string 'Garage2Context' not found.");

builder.Services.AddDbContext<Garage2Context>(options => options.UseSqlite("Data Source=Garage.db"));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Auto Migration: Update DB file and table automatically when the application starts.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<Garage2Context>();
        context.Database.EnsureCreated(); // Create DB file and table if they do not exist
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error occured during Database migration.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Error");
   // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ParkedVehicles}/{action=Index}/{id?}")
    .WithStaticAssets();

//Komment 
app.Run();
