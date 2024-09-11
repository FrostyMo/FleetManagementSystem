using Microsoft.AspNetCore.Mvc;
//using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FleetManagementSystem.Data;

namespace FleetManagementSystem.Controllers
{
    public class DriverController : Controller
    {
        private readonly FleetManagementDbContext _context;

        public DriverController(FleetManagementDbContext context)
        {
            _context = context;
        }

        // GET: Driver/Index
        public async Task<IActionResult> Index(string searchQuery)
        {
            return View(await _context.Drivers.ToListAsync());
        }
        public async Task<IActionResult> SearchDrivers(string searchQuery)
        {
            var drivers = from d in _context.Drivers
                          select d;

            // Perform case-insensitive search
            if (!String.IsNullOrEmpty(searchQuery))
            {
                drivers = drivers.Where(s => s.FirstName.ToLower().Contains(searchQuery.ToLower()) ||
                                             s.LastName.ToLower().Contains(searchQuery.ToLower()) ||
                                             s.LicenseNumber.ToLower().Contains(searchQuery.ToLower()));
            }

            // Return the partial view with the filtered drivers
            return PartialView("_DriverTable", await drivers.ToListAsync());
        }

        // GET: Driver/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Driver/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,LicenseNumber,DateOfBirth,LicenseExpiry,PhoneNumber,Email")] Driver driver)
        {
            if (ModelState.IsValid)
            {
                _context.Add(driver);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(driver);
        }

        // GET: Driver/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
            {
                return NotFound();
            }
            return View(driver);
        }

        // POST: Driver/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,LicenseNumber,DateOfBirth,LicenseExpiry,PhoneNumber,Email")] Driver driver)
        {
            if (id != driver.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(driver);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DriverExists(driver.Id))
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
            return View(driver);
        }

        // GET: Driver/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        // GET: Driver/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        // POST: Driver/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DriverExists(int id)
        {
            return _context.Drivers.Any(e => e.Id == id);
        }
    }
}