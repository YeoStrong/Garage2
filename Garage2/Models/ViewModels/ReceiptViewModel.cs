using Garage2.Models.Entities;

namespace Garage2.Models.ViewModels
{
    // ViewModel used to display a parking receipt after a vehicle is checked out.
    public class ReceiptViewModel
    {
        public string VehicleType { get; set; } = string.Empty;

        public string RegistrationNumber { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public int NumberOfWheels { get; set; }

        public DateTime ArrivalTime { get; set; }

        public DateTime CheckOutTime { get; set; }

        public TimeSpan ParkingDuration { get; set; }

        public decimal TotalPrice { get; set; }
    }
}