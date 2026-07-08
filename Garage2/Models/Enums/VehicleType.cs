using System.ComponentModel.DataAnnotations;

namespace Garage2.Models.Enums
{
    public enum VehicleType
    {
        [Display(Name = "🚗 Car", ShortName = "🚗")]
        Car,

        [Display(Name = "🏍 Motorcyclе", ShortName = "🏍")]
        Motorcycle,

        [Display(Name = "🚌 Bus", ShortName = "🚌")]
        Bus,

        [Display(Name = "🚚 Truck", ShortName = "🚚")]
        Truck,

        [Display(Name = "🚲 Bicycle", ShortName = "🚲")]
        Bicycle,

        [Display(Name = "✈️ Airplane", ShortName = "✈️")]
        Airplane,

        [Display(Name = "🚤 Boat", ShortName = "🚤")]
        Boat
    }
}