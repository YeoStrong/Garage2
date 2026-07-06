using Garage2.Models.Entities;
using Garage2.Models.Enums;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Garage2Context") ?? throw new InvalidOperationException("Connection string 'Garage2Context' not found.");

builder.Services.AddDbContext<Garage2Context>(options => options.UseSqlite("Data Source=Garage.db"));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IVehicleHandler, VehicleHandler>();

var app = builder.Build();

// Auto Migration: Update DB file and table automatically when the application starts.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<Garage2Context>();
        context.Database.EnsureCreated(); // Create DB file and table if they do not exist

        AddSeedData(context);
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


app.Run();


void AddSeedData(Garage2Context context)
{
    if (context.ParkedVehicles.Any())
    {
        return;
    }

    context.ParkedVehicles.AddRange(
    new ParkedVehicle
    {
        VehicleType = VehicleType.Car,
        RegistrationNumber = "ABC123",
        Color = "Black",
        Brand = "Volvo",
        Model = "XC60",
        NumberOfWheels = 4,
        ArrivalTime = DateTime.Now.AddHours(-3)
    },

    new ParkedVehicle
    {
        VehicleType = VehicleType.Motorcycle,
        RegistrationNumber = "KTM555",
        Color = "Orange",
        Brand = "KTM",
        Model = "Duke 390",
        NumberOfWheels = 2,
        ArrivalTime = DateTime.Now.AddDays(-1)
    },

    new ParkedVehicle
    {
        VehicleType = VehicleType.Bus,
        RegistrationNumber = "BUS010",
        Color = "Red",
        Brand = "Scania",
        Model = "Citywide",
        NumberOfWheels = 6,
        ArrivalTime = DateTime.Now.AddHours(-8)
    },

    new ParkedVehicle
    {
        VehicleType = VehicleType.Truck,
        RegistrationNumber = "TRK777",
        Color = "Blue",
        Brand = "Volvo",
        Model = "FH16",
        NumberOfWheels = 10,
        ArrivalTime = DateTime.Now.AddDays(-2)
    },

    new ParkedVehicle
    {
        VehicleType = VehicleType.Bicycle,
        RegistrationNumber = "BIK111",
        Color = "Yellow",
        Brand = "Crescent",
        Model = "Kebne",
        NumberOfWheels = 2,
        ArrivalTime = DateTime.Now.AddMinutes(-30)
    },

    new ParkedVehicle
    {
        VehicleType = VehicleType.Airplane,
        RegistrationNumber = "SAS901",
        Color = "White",
        Brand = "Airbus",
        Model = "A320neo",
        NumberOfWheels = 3,
        ArrivalTime = DateTime.Now.AddHours(-15)
    },

    new ParkedVehicle
    {
        VehicleType = VehicleType.Boat,
        RegistrationNumber = "BOA999",
        Color = "White",
        Brand = "Buster",
        Model = "Magnum",
        NumberOfWheels = 0,
        ArrivalTime = DateTime.Now.AddHours(-12)
    },
    new ParkedVehicle
    {
        VehicleType = VehicleType.Car,
        RegistrationNumber = "XYZ789",
        Color = "White",
        Brand = "Tesla",
        Model = "Model Y",
        NumberOfWheels = 4,
        ArrivalTime = DateTime.Now.AddHours(-5)
    },
    new ParkedVehicle
    {
        VehicleType = VehicleType.Car,
        RegistrationNumber = "MLB442",
        Color = "Grey",
        Brand = "Volkswagen",
        Model = "Golf",
        NumberOfWheels = 4,
        ArrivalTime = DateTime.Now.AddMinutes(-45)
    },
    new ParkedVehicle
    {
        VehicleType = VehicleType.Car,
        RegistrationNumber = "SWE999",
        Color = "Silver",
        Brand = "Polestar",
        Model = "Polestar 2",
        NumberOfWheels = 4,
        ArrivalTime = DateTime.Now.AddHours(-2)
    }
);

    context.SaveChanges();
}