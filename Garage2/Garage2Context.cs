using Garage2.Models.Entities;
using Microsoft.EntityFrameworkCore;

public class Garage2Context(DbContextOptions<Garage2Context> options) : DbContext(options)
{
    public DbSet<Garage2.Models.Entities.ParkedVehicle> ParkedVehicle { get; set; } = default!;

    public DbSet<ParkedVehicle> ParkedVehicles { get; set; }

}
