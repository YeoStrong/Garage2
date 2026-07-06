using Microsoft.EntityFrameworkCore;
using Garage2.Models.Entities;

namespace Garage2.Services
{
    public class VehicleHandler : IVehicleHandler
    {
        private readonly Garage2Context _context;

        public VehicleHandler(Garage2Context context)
        {
            _context = context;
        }

        public bool IsExisting(string regNumber)
        {
            return _context.ParkedVehicle.Any(v => v.RegistrationNumber == regNumber);
        }
    }
}