using Garage2.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Garage2.Models.ViewModels
{
    public class ParkedVehicleCreateViewModel
    {
        [Required(ErrorMessage = "Need registerationNumber.")]
        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; }
        [StringLength(20, ErrorMessage = "Color cannot be longer than 20 characters.")]
        [Display(Name = "Color (Max 20 Character)")]
        public string Color { get; set; }
        [StringLength(30, ErrorMessage = "Brand cannot be longer than 30 characters.")]
        [Display(Name = "Brand (Max 30 Character)")]
        public string Brand { get; set; }
        [StringLength(40, ErrorMessage = "Model cannot be longer than 40 characters.")]
        [Display(Name = "Model (Max 40 Character)")]
        public string Model { get; set; }
        [Range(0, 1000)]
        [Display(Name = "Number of Wheels")]
        public int NumberOfWheels { get; set; }
        [Display(Name = "Vehicle Type")]
        public VehicleType VehicleType { get; set; }
        public IEnumerable<SelectListItem>? VehicleTypes { get; set; }
    }
}
