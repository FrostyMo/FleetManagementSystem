using Microsoft.AspNetCore.Mvc;
using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FleetManagementSystem.Controllers
{
    public class InsuranceController : Controller
    {
        private readonly FleetManagementDbContext _context;

        public InsuranceController(FleetManagementDbContext context)
        {
            _context = context;
        }

        // GET: Insurance/Index
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var insuranceQuery = _context.Insurances.Include(i => i.Vehicles).Include(i => i.Drivers);
            var paginatedInsurance = await insuranceQuery.GetPagedAsync(page, pageSize);
            return View(paginatedInsurance);
        }


        // GET: Insurance/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id, int page = 1, int pageSize = 3)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the insurance policy along with related drivers and vehicles
            var insurance = await _context.Insurances
                                          .Include(i => i.Drivers)  // Eager load drivers
                                          .Include(i => i.Vehicles) // Eager load vehicles
                                          .FirstOrDefaultAsync(i => i.Id == id);

            if (insurance == null)
            {
                return NotFound();
            }

            // Pagination for associated vehicles or drivers if needed in the future
            // Can add similar logic for paginating Drivers or Vehicles, but for simplicity, all are included here

            return View(insurance);
        }

        // GET: Insurance/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.VehicleList = _context.Vehicles.Select(v => new SelectListItem
            {
                Value = v.Id.ToString(),
                Text = v.LicensePlate
            }).ToList();

            ViewBag.DriverList = _context.Drivers.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"{d.FirstName} {d.LastName}"
            }).ToList();

            return View(new Insurance());
        }

        // POST: Insurance/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Insurance insurance, IFormFile? policyDocument, List<int>? SelectedDrivers, List<int>? SelectedVehicles)
        {
            if (ModelState.IsValid)
            {

                if (policyDocument != null)
                {
                    // Ensure MOT directory exists
                    var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/PolicyDocuments");
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(policyDocument.FileName)}";
                    var filePath = Path.Combine(dir, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await policyDocument.CopyToAsync(stream);
                    }
                    insurance.PolicyDocuments = $"/PolicyDocuments/{uniqueFileName}";
                }

                // Attach the selected drivers and vehicles
                if (SelectedDrivers != null && SelectedDrivers.Count > 0)
                {
                    insurance.Drivers = await _context.Drivers
                                                       .Where(d => SelectedDrivers.Contains(d.Id))
                                                       .ToListAsync();
                }

                if (SelectedVehicles != null && SelectedVehicles.Count > 0)
                {
                    insurance.Vehicles = await _context.Vehicles
                                                       .Where(v => SelectedVehicles.Contains(v.Id))
                                                       .ToListAsync();
                }

                _context.Add(insurance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(nameof(Index));
        }

        // GET: Insurance/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var insurance = _context.Insurances
                .Include(i => i.Vehicles)
                .Include(i => i.Drivers)
                .FirstOrDefault(i => i.Id == id);
            if (insurance == null)
            {
                return NotFound();
            }

            ViewBag.VehicleList = _context.Vehicles.Select(v => new SelectListItem
            {
                Value = v.Id.ToString(),
                Text = v.LicensePlate
            }).ToList();

            ViewBag.DriverList = _context.Drivers.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"{d.FirstName} {d.LastName}"
            }).ToList();

            return View(insurance);
        }

        // POST: Insurance/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Insurance insurance, IFormFile? policyDocument, List<int>? SelectedDrivers, List<int>? SelectedVehicles)
        {
            if (id != insurance.Id)
            {
                return NotFound();
            }
            var existing = await _context.Insurances
                            .Include(i => i.Vehicles)
                            .Include(i => i.Drivers)
                            .FirstOrDefaultAsync(i => i.Id == id);
            if (existing == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (policyDocument != null)
                    {
                        // Ensure MOT directory exists
                        var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/PolicyDocuments");
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }

                        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(policyDocument.FileName)}";
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/PolicyDocuments", uniqueFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await policyDocument.CopyToAsync(stream);
                        }
                        insurance.PolicyDocuments = "/PolicyDocuments/" + uniqueFileName;
                    }
                    else
                    {
                        insurance.PolicyDocuments = existing.PolicyDocuments;
                    }
                    

                    _context.Entry(existing).CurrentValues.SetValues(insurance);
                    // Update Drivers - Change Tracking will manage which are added or removed
                    if (SelectedDrivers != null)
                    {
                        var newDrivers = await _context.Drivers.Where(d => SelectedDrivers.Contains(d.Id)).ToListAsync();
                        existing.Drivers?.Clear();
                        existing.Drivers?.AddRange(newDrivers);
                    }

                    // Update Vehicles - Change Tracking will manage which are added or removed
                    if (SelectedVehicles != null)
                    {
                        var newVehicles = await _context.Vehicles.Where(v => SelectedVehicles.Contains(v.Id)).ToListAsync();
                        existing.Vehicles?.Clear();
                        existing.Vehicles?.AddRange(newVehicles);
                    }

                    // Save all changes
                    _context.Update(existing);  // EF Core will track all changes
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Insurances.Any(e => e.Id == insurance.Id))
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

            return View(insurance);
        }

        // GET: Insurance/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Include Drivers and Vehicles in the query to ensure they're loaded
            var insurance = await _context.Insurances
                                          .Include(i => i.Drivers)   // Eagerly load Drivers
                                          .Include(i => i.Vehicles)  // Eagerly load Vehicles
                                          .FirstOrDefaultAsync(m => m.Id == id);
            if (insurance == null)
            {
                return NotFound();
            }

            return View(insurance);
        }

        // POST: Insurance/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var insurance = await _context.Insurances.FindAsync(id);
            if (insurance == null)
            {
                return NotFound();
            }

            _context.Insurances.Remove(insurance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: SearchDrivers
        [HttpGet]
        public async Task<IActionResult> SearchDrivers(string searchString)
        {
            
            var results = await _context.Drivers
                                        .Where(d => d.FirstName.ToLower().Contains(searchString.ToLower()) || d.LastName.ToLower().Contains(searchString.ToLower()))
                                        .Select(d => new { d.Id, Name = d.FirstName + " " + d.LastName })
                                        .Take(10) // Limit the number of results
                                        .ToListAsync();

            return Json(results);
        }

        // GET: SearchVehicles
        [HttpGet]
        public async Task<IActionResult> SearchVehicles(string searchString)
        {
            var results = await _context.Vehicles
                                        .Where(v => v.LicensePlate.ToLower().Contains(searchString.ToLower()))
                                        .Select(v => new { v.Id, v.LicensePlate })
                                        .Take(10) // Limit the number of results
                                        .ToListAsync();

            return Json(results);
        }


    }
}