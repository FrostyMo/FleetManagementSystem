using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using System.Threading.Tasks;
using System.Linq;

namespace FleetManagementSystem.Controllers
{
    public class FuelCardController : Controller
    {
        private readonly FleetManagementDbContext _context;

        public FuelCardController(FleetManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var fuelCards = _context.FuelCards
                            .Include(fc => fc.Driver) // Include Driver if necessary
                            .Include(fc => fc.FuelCardDetails) // Include FuelCardDetails
                            .AsQueryable();
            var paginatedResult = await fuelCards.GetPagedAsync(page, pageSize); // Use pagination extension method
            return View(paginatedResult); // Ensure the returned model is a PagedResult<FuelCard>
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchString, int page = 1, int pageSize = 10)
        {
            var fuelCards = _context.FuelCards
                             .Include(fc => fc.Driver) // Include Driver if necessary
                             .Include(fc => fc.FuelCardDetails) // Include FuelCardDetails
                             .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                fuelCards = fuelCards.Where(fc =>
                    fc.FuelCardNumber.ToLower().Contains(searchString) ||
                    fc.Driver.FirstName.ToLower().Contains(searchString) ||
                    fc.Driver.LastName.ToLower().Contains(searchString));
            }

            var paginatedResult = await fuelCards.GetPagedAsync(page, pageSize); // Get paginated result
            return PartialView("_FuelCardTablePartial", paginatedResult); // Ensure returning PagedResult<FuelCard>
        }

        // GET: FuelCard/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.DriverList = _context.Drivers.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"{d.FirstName} {d.LastName}"
            }).ToList();

            return View();
        }

        // GET: FuelCard/Details/5
        public async Task<IActionResult> Details(int? id, int page = 1, int pageSize = 3)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the fuel card
            var fuelCard = await _context.FuelCards
                .Include(fc => fc.Driver) // Include driver info
                .FirstOrDefaultAsync(fc => fc.Id == id);

            if (fuelCard == null)
            {
                return NotFound();
            }

            // Fetch FuelCardDetails for this fuel card and apply pagination
            var fuelCardDetailsQuery = _context.FuelCardDetails
                .Where(fcd => fcd.FuelCardId == id)
                .OrderByDescending(fcd => fcd.Year) // Order by month-year
                .ThenByDescending(fcd => fcd.Month) // Order by month-year
                .AsQueryable();

            var paginatedDetails = await fuelCardDetailsQuery.GetPagedAsync(page, pageSize); // Assuming you have a GetPagedAsync() method
            paginatedDetails.Action = "Details"; // Specify the action name for pagination
            ViewBag.FuelCardDetails = paginatedDetails; // Pass paginated details to the view

            return View(fuelCard);
        }

        // POST: FuelCard/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FuelCard fuelCard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fuelCard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DriverList = _context.Drivers.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"{d.FirstName} {d.LastName}"
            }).ToList();
            return View(fuelCard);
        }

        // GET: FuelCard/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var fuelCard = await _context.FuelCards.FindAsync(id);
            if (fuelCard == null) return NotFound();

            ViewBag.DriverList = new SelectList(await _context.Drivers.ToListAsync(), "Id", "FirstName", fuelCard.DriverId);
            return View(fuelCard);
        }

        // POST: FuelCard/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FuelCard fuelCard)
        {
            if (id != fuelCard.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fuelCard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FuelCardExists(fuelCard.Id))
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

            ViewBag.DriverList = new SelectList(await _context.Drivers.ToListAsync(), "Id", "FirstName", fuelCard.DriverId);
            return View(fuelCard);
        }

        // GET: FuelCard/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var fuelCard = await _context.FuelCards
                .Include(fc => fc.Driver)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (fuelCard == null) return NotFound();

            // Check if the card has any associated details
            var hasDetails = await _context.FuelCardDetails.AnyAsync(f => f.FuelCardId == fuelCard.Id);

            // Build the error message dynamically
            var errorMessages = new List<string>();

            if (hasDetails)
            {
                errorMessages.Add("&#8627; This fuel card has fuel card details associated with it. You must delete them before deleting the card.");
            }
            // Join error messages with <br> for line breaks in HTML
            ViewBag.ErrorMessage = string.Join("<br>", errorMessages);

            return View(fuelCard);
        }

        // POST: FuelCard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fuelCard = await _context.FuelCards.FindAsync(id);
            _context.FuelCards.Remove(fuelCard);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FuelCardExists(int id)
        {
            return _context.FuelCards.Any(e => e.Id == id);
        }
    }
}