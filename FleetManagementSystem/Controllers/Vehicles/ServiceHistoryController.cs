﻿using Microsoft.AspNetCore.Mvc;
using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ServiceHistoryController : Controller
{
    private readonly FleetManagementDbContext _context;

    public ServiceHistoryController(FleetManagementDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string searchString)
    {
        var histories = _context.ServiceHistories.
                        Include(h => h.Vehicle).
                        AsQueryable();
        return View(await histories.ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Search(string searchString)
    {
        var histories = _context.ServiceHistories.
                        Include(h => h.Vehicle).
                        AsQueryable();

        searchString = searchString.ToLower();
        if (!string.IsNullOrEmpty(searchString))
        {
            histories = histories.Where(v => v.Type.ToLower().StartsWith(searchString) ||
                                            v.Vehicle.LicensePlate.ToLower().StartsWith(searchString)
                                            );
        }

        return PartialView("_ServiceHistoryTablePartial", await histories.ToListAsync());
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.VehicleList = new SelectList(_context.Vehicles, "Id", "LicensePlate");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceHistory serviceHistory)
    {
        if (ModelState.IsValid)
        {
            // Ensure that the VehicleId is assigned and corresponds to a valid vehicle
            if (serviceHistory.VehicleId != null)
            {
                var vehicle = await _context.Vehicles.FindAsync(serviceHistory.VehicleId);
                if (vehicle == null)
                {
                    ModelState.AddModelError("VehicleId", "Invalid vehicle selected.");
                    ViewBag.VehicleList = new SelectList(_context.Vehicles, "Id", "LicensePlate", serviceHistory.VehicleId);
                    return View(serviceHistory); // Show error in view
                }

                serviceHistory.Vehicle = vehicle;
            }

            _context.Add(serviceHistory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // If the model state is invalid, re-populate the vehicle dropdown and return the form
        ViewBag.VehicleList = new SelectList(_context.Vehicles, "Id", "LicensePlate", serviceHistory.VehicleId);
        return View(serviceHistory);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var serviceHistory = await _context.ServiceHistories.FindAsync(id);
        if (serviceHistory == null)
        {
            return NotFound();
        }
        ViewBag.VehicleList = new SelectList(await _context.Vehicles.ToListAsync(), "Id", "LicensePlate", serviceHistory.Id);
        return View(serviceHistory);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ServiceHistory serviceHistory)
    {
        if (id != serviceHistory.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(serviceHistory);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceHistoryExists(serviceHistory.Id))
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
        return View(serviceHistory);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var serviceHistory = await _context.ServiceHistories.FirstOrDefaultAsync(m => m.Id == id);
        if (serviceHistory == null)
        {
            return NotFound();
        }

        return View(serviceHistory);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var serviceHistory = await _context.ServiceHistories.FindAsync(id);
        _context.ServiceHistories.Remove(serviceHistory);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ServiceHistoryExists(int id)
    {
        return _context.ServiceHistories.Any(e => e.Id == id);
    }
}