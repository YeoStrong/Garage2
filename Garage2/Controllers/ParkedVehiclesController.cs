
using Garage2.Models.Entities;
using Garage2.Models.Enums;
using Garage2.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garage2.Models.Entities;
using Garage2.Models.ViewModels;
public class ParkedVehiclesController : Controller
{
    private readonly Garage2Context _context;
    private readonly IVehicleHandler _vehicleHandler;

    public ParkedVehiclesController(Garage2Context context, IVehicleHandler vehicleHandler)
    {
        _context = context;
        _vehicleHandler = vehicleHandler;
    }

    // GET: PARKEDVEHICLES
    public async Task<IActionResult> Index()
    {
        return View(await _context.ParkedVehicle.ToListAsync());
    }

    // GET: PARKEDVEHICLES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var parkedvehicle = await _context.ParkedVehicle
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
                                         Value = v.ToString()
                                     });
        return View(viewModel);
    }

    // POST: PARKEDVEHICLES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ParkedVehicleFormViewModel viewModel)
    {
        bool regExists = await _context.ParkedVehicles.AnyAsync(v => v.RegistrationNumber == viewModel.RegistrationNumber);

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
                    NumberOfWheels = viewModel.NumberOfWheels,
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
                Text = v.GetDisplayName(),
                Value = v.ToString()
            });

        return View(viewModel);
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
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, ParkedVehicleFormViewModel vm)
    {
        if (id != vm.Id)
        {
            return NotFound();
        }

        // Hämta originalet från databasen
        var original = await _context.ParkedVehicles.FindAsync(id);

        if (original == null) { return NotFound(); }

        // Kontrollera endast om användaren ändrade registreringsnumret
        if (original.RegistrationNumber != vm.RegistrationNumber)
        {
            bool regExists = await _context.ParkedVehicles.AnyAsync(v => v.RegistrationNumber == vm.RegistrationNumber);

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
                original.NumberOfWheels = vm.NumberOfWheels;

                // _context.Update(original);
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

        var parkedvehicle = await _context.ParkedVehicle
            .FirstOrDefaultAsync(m => m.Id == id);

        if (parkedvehicle == null)
        {
            return NotFound();
        }

        var viewModel = new CheckOutViewModel
        {
            Id = parkedvehicle.Id,
            RegistrationNumber = parkedvehicle.RegistrationNumber,
            VehicleType = parkedvehicle.VehicleType.ToString(),
            ArrivalTime = parkedvehicle.ArrivalTime
        };

        return View(viewModel);
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

        // Save checkout information
        DateTime checkOutTime = DateTime.Now;

        // Calculate parking duration
        TimeSpan parkingDuration = checkOutTime - parkedvehicle.ArrivalTime;
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
            ParkingDuration = parkingDuration,
            TotalPrice = 0
        };

        _context.ParkedVehicle.Remove(parkedvehicle);

        await _context.SaveChangesAsync();

        return View("Receipt", receiptViewModel);
    }

    private bool ParkedVehicleExists(int? id)
    {
        return _context.ParkedVehicle.Any(e => e.Id == id);
    }
}
