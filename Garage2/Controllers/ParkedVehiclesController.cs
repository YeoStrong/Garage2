
using Microsoft.AspNetCore.Mvc;
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
        return View();
    }

    // POST: PARKEDVEHICLES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,VehicleType,RegistrationNumber,Color,Brand,Model,NumberOfWheels,ArrivalTime")] ParkedVehicle parkedvehicle)
    {
        if (ModelState.IsValid)
        {
            _context.Add(parkedvehicle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(parkedvehicle);
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
        return View(parkedvehicle);
    }

    // POST: PARKEDVEHICLES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,VehicleType,RegistrationNumber,Color,Brand,Model,NumberOfWheels,ArrivalTime")] ParkedVehicle parkedvehicle)
    {
        if (id != parkedvehicle.Id)
        {
            return NotFound();
        }

        // Hämta originalet från databasen
        var original = await _context.ParkedVehicles.AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);

        if (original == null) { return NotFound(); }

        // Kontrollera endast om användaren ändrade registreringsnumret
        if (original.RegistrationNumber != parkedvehicle.RegistrationNumber)
        {
            bool regExists = await _context.ParkedVehicles.AnyAsync(v => v.RegistrationNumber == parkedvehicle.RegistrationNumber);

            if (regExists)
            {
                ModelState.AddModelError("RegistrationNumber", "The registration number already exists. Please enter a different one.");
            }
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(parkedvehicle);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParkedVehicleExists(parkedvehicle.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(parkedvehicle);
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
