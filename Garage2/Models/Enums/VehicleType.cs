using System.ComponentModel.DataAnnotations;

namespace Garage2.Models.Enums
{
    public enum VehicleType
    {
        [Display(Name = "🚗 Car")]
        Car,

        [Display(Name = "🏍 Motorcyclе")]
        Motorcycle,

        [Display(Name = "🚌 Bus")]
        Bus,

        [Display(Name = "🚚 Truck ")]
        Truck,

        [Display(Name = "🚲 Bicycle")]
        Bicycle,

        [Display(Name = "✈️ Airplane")]
        Airplane,

        [Display(Name = "🚤 Boat")]
        Boat
    }
}