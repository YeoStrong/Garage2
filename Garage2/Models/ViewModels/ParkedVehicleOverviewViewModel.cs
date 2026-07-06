namespace Garage2.Models.ViewModels
{
    public class ParkedVehicleOverviewViewModel
    {
        public int Id { get; set; }
        public Garage2.Models.Enums.VehicleType VehicleType { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;
        public DateTime ArrivalTime { get; set; }
    }
}