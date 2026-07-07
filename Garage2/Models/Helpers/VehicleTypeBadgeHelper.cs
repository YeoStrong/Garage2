using Garage2.Models.Enums;

public static class VehicleTypeBadgeHelper
{
    public static string GetBadgeColor(VehicleType type) =>
        type switch
        {
            VehicleType.Car        => "#006AA7",
            VehicleType.Motorcycle => "#FECC02",
            VehicleType.Truck      => "#2c3e50",
            VehicleType.Bus        => "#1a7a4c",
            VehicleType.Boat       => "#0891b2",
            VehicleType.Airplane   => "#6b7280",
            _                      => "#6c757d"
        };

    public static string GetBadgeTextColor(VehicleType type) =>
        type == VehicleType.Motorcycle ? "#1a1a1a" : "#ffffff";
}
