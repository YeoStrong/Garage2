using Garage2.Models.Parking;
using Garage2.Models.Enums;

namespace Garage2.Helpers
{
    public static class SpotHelper
    {
        public static string GetSpotDisplay(int? assignedSpotNumber, VehicleType vehicleType)
        {
            if (!assignedSpotNumber.HasValue)
                return "Not assigned";

            int spotSpan = VehicleSpotRequirement.GetRequiredWholeSpots(vehicleType);
            int start = assignedSpotNumber.Value;
            int end = start + spotSpan - 1;

            return spotSpan > 1
                ? $"#{start} - #{end}"
                : $"#{start}";
        }
    }
}
