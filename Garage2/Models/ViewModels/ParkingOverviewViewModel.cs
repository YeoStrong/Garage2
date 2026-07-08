using Garage2.Models.Parking;

namespace Garage2.Models.ViewModels
{
    public class ParkingOverviewViewModel
    {
        public int TotalSpots { get; set; }

        public int FreeSpotCount { get; set; }

        public IReadOnlyList<ParkingSpotInfo> Spots { get; set; } = new List<ParkingSpotInfo>();
    }
}