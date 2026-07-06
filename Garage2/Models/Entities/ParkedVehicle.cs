using Garage2.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Garage2.Models.Entities
{
    [Index(nameof(RegistrationNumber), IsUnique = true)]
    public class ParkedVehicle
    {
        public int Id { get; set; }
        public VehicleType VehicleType { get; set; }
        public string RegistrationNumber { get; set; }
        public string Color { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int NumberOfWheels { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}
