using Microsoft.AspNetCore.Mvc;
//using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FleetManagementSystem.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var drivers = _context.Drivers.AsQueryable();
            var paginatedResult = await drivers.GetPagedAsync(page, pageSize);
            paginatedResult.Action = "Index";  // Specify the action name here
            return View(paginatedResult);
            //return View(await _context.Drivers.ToListAsync());
        }
        public async Task<IActionResult> SearchDrivers(string searchQuery, int page = 1, int pageSize = 10)
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
            var paginatedResult = await drivers.GetPagedAsync(page, pageSize);
            paginatedResult.Action = "Index";

            return PartialView("_DriverTable", paginatedResult);
            // Return the partial view with the filtered drivers
            //return PartialView("_DriverTable", await drivers.ToListAsync());
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
        public async Task<IActionResult> Edit(int id, Driver driver)
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
        public async Task<IActionResult> Details(int? id, int page = 1, int pageSize = 3)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers
                .Include(d => d.FuelCard) // Include FuelCard details
                .FirstOrDefaultAsync(m => m.Id == id);

            if (driver == null)
            {
                return NotFound();
            }

            // Fetch the fines for this driver and apply pagination
            var finesQuery = _context.Fines
                                    .Where(f => f.DriverId == id)
                                    .OrderBy(f => f.DateIssued)
                                    .AsQueryable();

            var paginatedFines = await finesQuery.GetPagedAsync(page, pageSize); // PagedResult
            paginatedFines.Action = "Details";
            ViewBag.Fines = paginatedFines; // Pass the paginated fines to the view

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

            // Check if the driver has any associated fines
            var hasFines = await _context.Fines.AnyAsync(f => f.DriverId == driver.Id);
            // Check if the driver has any associated fines
            var hasFuelCards = await _context.FuelCards.AnyAsync(f => f.DriverId == driver.Id);

            // Build the error message dynamically
            var errorMessages = new List<string>();

            if (hasFines)
            {
                errorMessages.Add("&#8627; This driver has fines associated with them. You must delete the fines before deleting the driver.");
            }
            if (hasFuelCards)
            {
                errorMessages.Add("&#8627; This driver has fuel cards associated with them. You must delete them before deleting the driver.");
            }

            // Join error messages with <br> for line breaks in HTML
            ViewBag.ErrorMessage = string.Join("<br>", errorMessages);

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

        [HttpPost]
        public async Task<IActionResult> UpdateStatusToPaid(int id)
        {
            var fine = await _context.Fines.FindAsync(id);
            if (fine == null){return NotFound();}

            // Check if there's proof of payment before allowing status change to "Paid"
            if (string.IsNullOrEmpty(fine.ProofOfPaymentPath))
            {
                TempData["ErrorMessage"] = "Cannot mark fine as paid without proof of payment.";
                return RedirectToAction(nameof(Details), new { id = fine.DriverId });
            }

            // Toggle the status (Paid or Pending)
            fine.IsPaid = !fine.IsPaid;
            _context.Update(fine);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = fine.DriverId });
        }
    }
}