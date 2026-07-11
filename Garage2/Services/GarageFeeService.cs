using Garage2.Models.Enums;

namespace Garage2.Services
{
    public class GarageFeeService
    {
        public decimal GetDayRate(VehicleType vehicleType)
        {
            return vehicleType switch
            {
                VehicleType.Truck => 40m,
                VehicleType.Bus => 40m,

                VehicleType.Boat => 60m,
                VehicleType.Airplane => 60m,

                _ => 20m
            };
        }

        public decimal GetNightRate(VehicleType vehicleType)
        {
            return vehicleType switch
            {
                VehicleType.Truck => 4m,
                VehicleType.Bus => 4m,

                VehicleType.Boat => 6m,
                VehicleType.Airplane => 6m,

                _ => 2m
            };
        }

        public decimal CalculateFee(
            VehicleType vehicleType,
            DateTime arrivalTime,
            DateTime departureTime)
        {
            decimal totalFee = 0m;

            DateTime currentTime = arrivalTime;

            while (currentTime < departureTime)
            {
                // Söndagar är gratis
                if (currentTime.DayOfWeek != DayOfWeek.Sunday)
                {
                    if (currentTime.Hour >= 6 && currentTime.Hour < 20)
                    {
                        totalFee += GetDayRate(vehicleType) / 60m;
                    }
                    else
                    {
                        totalFee += GetNightRate(vehicleType) / 60m;
                    }
                }

                currentTime = currentTime.AddMinutes(1);
            }

            return Math.Round(totalFee, 2);
        }

        public string GetPriceInfo(VehicleType vehicleType)
        {
            return $"{GetDayRate(vehicleType)} kr/timme (dag), {GetNightRate(vehicleType)} kr/timme (natt)";
        }
    }
}