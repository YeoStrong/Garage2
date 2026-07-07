using Garage2.Models.Entities;
using Garage2.Models.Enums;
using Garage2.Models.ViewModels;
using Garage2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

public class ParkedVehiclesController : Controller
{
    private readonly Garage2Context _context;
    private readonly IVehicleHandler _vehicleHandler;
    private readonly GarageFeeService _garageFeeService;

    public ParkedVehiclesController(Garage2Context context, IVehicleHandler vehicleHandler, GarageFeeService garageFeeService)
    {
        _context = context;
        _vehicleHandler = vehicleHandler;
        _garageFeeService = garageFeeService;
    }

    // GET: PARKEDVEHICLES
    public async Task<IActionResult> Index(string searchString, string sortOrder, string searchTime)
    {
        var vehicleQuery = _context.ParkedVehicle.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            searchString= searchString.Trim().ToLower();

            vehicleQuery = vehicleQuery.Where(
                v => v.RegistrationNumber.ToLower().Contains(searchString) || 
                v.VehicleType.ToString().ToLower().Contains(searchString)
            );
        }

        switch (sortOrder)
        {
            case "RegAsc":
                vehicleQuery = vehicleQuery.OrderBy(v => v.RegistrationNumber);
                break;

            case "RegDesc":
                vehicleQuery = vehicleQuery.OrderByDescending(v => v.RegistrationNumber);
                break;

            case "TypeAsc":
                vehicleQuery = vehicleQuery.OrderBy(v => v.VehicleType);
                break;

            case "TypeDesc":
                vehicleQuery = vehicleQuery.OrderByDescending(v => v.VehicleType);
                break;

            case "DateAsc":
                vehicleQuery = vehicleQuery.OrderBy(v => v.ArrivalTime);
                break;

            case "DateDesc":
                vehicleQuery = vehicleQuery.OrderByDescending(v => v.ArrivalTime);
                break;

            case "DurationAsc":
                vehicleQuery = vehicleQuery.OrderByDescending(v => v.ArrivalTime);
                break;

            case "DurationDesc":
                vehicleQuery = vehicleQuery.OrderBy(v => v.ArrivalTime);
                break;

            default:
                vehicleQuery = vehicleQuery.OrderBy(v => v.RegistrationNumber);
                break;

        }

        var vehicles = await vehicleQuery
            .Select(v => new ParkedVehicleOverviewViewModel
            {
                Id = v.Id,
                VehicleType = v.VehicleType,
                RegistrationNumber = v.RegistrationNumber,
                ArrivalTime = v.ArrivalTime
            })
            .ToListAsync();

        if (!string.IsNullOrEmpty(searchTime)) {
            searchTime = searchTime.Trim().ToLower();

            bool isDate = DateTime.TryParse(searchTime, out DateTime parsedDate);
            bool isYear = int.TryParse(searchTime, out int year);
            bool isDay = int.TryParse(searchTime, out int day);

            bool isNumber = int.TryParse(searchTime, out int number);

            var months = new[]
            {
                "january","february","march","april","may","june",
                "july","august","september","october","november","december"
            };
            bool isMonthName = months.Any(m => m.Contains(searchTime));

            var weeks = new[]
            {
                "sunday", "monday","tuesday","wednesday","thursday","friday","saturday"
            };
            bool isWeekName = weeks.Any(w => w.Contains(searchTime));

            bool isDouble = double.TryParse(searchTime, out double doubleNumber);

            vehicles = vehicles.Where(v =>
                FormatDuration(v.ArrivalTime).Contains(searchTime) ||
                (isDate && v.ArrivalTime.Date == parsedDate.Date) ||
                (isYear && v.ArrivalTime.Year == year) ||
                (isDay && v.ArrivalTime.Day == day) ||
                (isNumber && v.ArrivalTime.Month == number) ||
                (isMonthName && months[v.ArrivalTime.Month - 1].Contains(searchTime)) ||
                (isWeekName && weeks[(int)v.ArrivalTime.DayOfWeek].Contains(searchTime)) ||
                v.ArrivalTime.Minute.ToString().Contains(searchTime) ||
                v.ArrivalTime.Hour.ToString().Contains(searchTime)
            ).ToList();
        }

        ViewData["CurrentFilter"] = searchString;
        ViewData["CurrentTimeFilter"] = searchTime;

        ViewData["RegSortParm"] = (string.IsNullOrEmpty(sortOrder) || sortOrder == "RegAsc") ? "RegDesc" : "RegAsc";
        ViewData["TypeSortParm"] = sortOrder == "TypeAsc" ? "TypeDesc" : "TypeAsc";
        ViewData["DateSortParm"] = sortOrder == "DateAsc" ? "DateDesc" : "DateAsc";
        ViewData["DurationSortParm"] = sortOrder == "DurationAsc" ? "DurationDesc" : "DurationAsc";
        ViewData["CurrentSort"] = sortOrder;

        return View(vehicles);
    }

    // GET: PARKEDVEHICLES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var parkedvehicle = await _context.ParkedVehicle.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (parkedvehicle == null)
        {
            return NotFound();
        }

        return View(parkedvehicle);
    }

    // GET: PARKEDVEHICLES/Create
    public IActionResult Create()
    {
        var viewModel = new ParkedVehicleFormViewModel();

        viewModel.VehicleTypes = Enum.GetValues(typeof(VehicleType))
                                     .Cast<VehicleType>()
                                     .Select(v => new SelectListItem
                                    {
                                        Text = v.GetDisplayName(),
                                        Value = ((int)v).ToString()
                                     });
        return View(viewModel);
    }

    // POST: PARKEDVEHICLES/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ParkedVehicleFormViewModel viewModel)
    {
        bool regExists = await _context.ParkedVehicle.AnyAsync(v => v.RegistrationNumber == viewModel.RegistrationNumber);

        if (regExists)
        {
            ModelState.AddModelError("RegistrationNumber", "The registration number already exists. Please enter a different one.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                var parkedvehicle = new ParkedVehicle
                {
                    VehicleType = viewModel.VehicleType,
                    RegistrationNumber = viewModel.RegistrationNumber,
                    Color = viewModel.Color ?? string.Empty,
                    Brand = viewModel.Brand ?? string.Empty,
                    Model = viewModel.Model ?? string.Empty,
                    NumberOfWheels = viewModel.NumberOfWheels.Value,
                    ArrivalTime = DateTime.Now
                };

                _context.Add(parkedvehicle);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Successfully checked in {viewModel.RegistrationNumber}.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Could not check in vehicle. Please check all fields.");
                Console.WriteLine("DB ERROR: " + ex.Message);
            }
        }

        viewModel.VehicleTypes = Enum.GetValues(typeof(VehicleType))
                                     .Cast<VehicleType>()
                                     .Select(v => new SelectListItem
                                     {
                                         Text = v.ToString(),
                                         Value = v.ToString()
                                     });

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult CheckDuplicate(string registrationNumber, int? id)
    {
        // If editing, we don't want to check for duplicates against the same record
        bool isDuplicate = id.HasValue
            ? false
            : _context.ParkedVehicles.Any(v => v.RegistrationNumber == registrationNumber);
        return Json(!isDuplicate);
    }

    // GET: PARKEDVEHICLES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var parkedvehicle = await _context.ParkedVehicle.FindAsync(id);
        if (parkedvehicle == null)
        {
            return NotFound();
        }

        var vm = new ParkedVehicleFormViewModel
        {
            Id = parkedvehicle.Id,
            RegistrationNumber = parkedvehicle.RegistrationNumber,
            VehicleType = parkedvehicle.VehicleType,
            Color = parkedvehicle.Color,
            Brand = parkedvehicle.Brand,
            Model = parkedvehicle.Model,
            NumberOfWheels = parkedvehicle.NumberOfWheels,
            ArrivalTime = parkedvehicle.ArrivalTime,

            VehicleTypes = Enum.GetValues(typeof(VehicleType))
                .Cast<VehicleType>()
                .Select(v => new SelectListItem
                {
                    Text = v.GetDisplayName(),
                    Value = v.ToString()
                })
        };

        return View(vm);
    }

    // POST: PARKEDVEHICLES/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, ParkedVehicleFormViewModel vm)
    {
        if (id != vm.Id)
        {
            return NotFound();
        }

        var original = await _context.ParkedVehicle.AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);

        if (original == null) { return NotFound(); }

        if (original.RegistrationNumber != vm.RegistrationNumber)
        {
            bool regExists = await _context.ParkedVehicle.AnyAsync(v => v.RegistrationNumber == vm.RegistrationNumber);

            if (regExists)
            {
                ModelState.AddModelError("RegistrationNumber", "The registration number already exists. Please enter a different one.");
            }
        }

        if (ModelState.IsValid)
        {
            try
            {
                original.VehicleType = vm.VehicleType;
                original.RegistrationNumber = vm.RegistrationNumber;
                original.Color = vm.Color ?? string.Empty;
                original.Brand = vm.Brand ?? string.Empty;
                original.Model = vm.Model ?? string.Empty;
                original.NumberOfWheels = vm.NumberOfWheels.Value;

                _context.Update(original);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Successfully saved changes to {vm.RegistrationNumber}.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Could not save changes. Please check all fields.");
                Console.WriteLine("DB ERROR: " + ex.Message);
            }
        }

        vm.VehicleTypes = Enum.GetValues(typeof(VehicleType))
            .Cast<VehicleType>()
            .Select(v => new SelectListItem
            {
                Text = v.GetDisplayName(),
                Value = v.ToString()
            });

        return View(vm);
    }

    // GET: PARKEDVEHICLES/CheckOut/5
    public async Task<IActionResult> CheckOut(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var parkedvehicle = await _context.ParkedVehicle.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (parkedvehicle == null)
        {
            return NotFound();
        }

        /* var viewModel = new CheckOutViewModel
        {
            Id = parkedvehicle.Id,
            RegistrationNumber = parkedvehicle.RegistrationNumber,
            VehicleType = parkedvehicle.VehicleType.ToString(),
            ArrivalTime = parkedvehicle.ArrivalTime
        }; */

        return View(parkedvehicle);
    }

    // POST: PARKEDVEHICLES/CheckOut/5
    [HttpPost, ActionName("CheckOut")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckOutConfirmed(int? id)
    {
        var parkedvehicle = await _context.ParkedVehicle.FindAsync(id);

        if (parkedvehicle == null)
        {
            return NotFound();
        }

        DateTime checkOutTime = DateTime.Now;

        // Create the receipt data that will be displayed after check out
        var receiptViewModel = new ReceiptViewModel
        {
            VehicleType = parkedvehicle.VehicleType.ToString(),
            RegistrationNumber = parkedvehicle.RegistrationNumber,
            Brand = parkedvehicle.Brand,
            Model = parkedvehicle.Model,
            Color = parkedvehicle.Color,
            NumberOfWheels = parkedvehicle.NumberOfWheels,
            ArrivalTime = parkedvehicle.ArrivalTime,
            CheckOutTime = checkOutTime,

            TotalPrice = _garageFeeService.CalculateFee(
                parkedvehicle.ArrivalTime,
                checkOutTime)
        };

        _context.ParkedVehicle.Remove(parkedvehicle);

        await _context.SaveChangesAsync();

        TempData["Receipt"] = JsonSerializer.Serialize(receiptViewModel);

        TempData["SuccessMessage"] = $"Successfully checked out {receiptViewModel.RegistrationNumber}.";

        return RedirectToAction(nameof(Receipt));
    }

    // GET: PARKEDVEHICLES/Receipt
    public IActionResult Receipt()
    {
        if (TempData["Receipt"] is not string json)
        {
            return RedirectToAction(nameof(Index));
        }

        var receipt = JsonSerializer.Deserialize<ReceiptViewModel>(json);

        return View(receipt);
    }

    private string FormatDuration(DateTime arrival)
    {
        var span = DateTime.Now - arrival;
        int days = (int)span.TotalDays;
        int hours = span.Hours;
        int minutes = span.Minutes;

        return $"{days}d {hours}h {minutes}m , {days} d {hours} h {minutes} m";
    }
}