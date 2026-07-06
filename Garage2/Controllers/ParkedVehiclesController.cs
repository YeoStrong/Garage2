using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Garage2.Models.Entities;
using Garage2.Models.Enums;
using Garage2.Models.ViewModels;
using Garage2.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

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
        var vehicles = await _context.ParkedVehicle
            .Select(v => new ParkedVehicleOverviewViewModel
            {
                Id = v.Id,
                VehicleType = v.VehicleType,
                RegistrationNumber = v.RegistrationNumber,
                ArrivalTime = v.ArrivalTime
            })
            .ToListAsync();

        return View(vehicles);
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
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,VehicleType,RegistrationNumber,Color,Brand,Model,NumberOfWheels,ArrivalTime")] ParkedVehicle parkedvehicle)
    {
        if (id != parkedvehicle.Id)
        {
            return NotFound();
        }

        var original = await _context.ParkedVehicle.AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);

        if (original == null) { return NotFound(); }

        if (original.RegistrationNumber != parkedvehicle.RegistrationNumber)
        {
            bool regExists = await _context.ParkedVehicle.AnyAsync(v => v.RegistrationNumber == parkedvehicle.RegistrationNumber);

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

    // GET: PARKEDVEHICLES/Delete/5
    public async Task<IActionResult> Delete(int? id)
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

    // POST: PARKEDVEHICLES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var parkedvehicle = await _context.ParkedVehicle.FindAsync(id);
        if (parkedvehicle != null)
        {
            _context.ParkedVehicle.Remove(parkedvehicle);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ParkedVehicleExists(int? id)
    {
        return _context.ParkedVehicle.Any(e => e.Id == id);
    }
}