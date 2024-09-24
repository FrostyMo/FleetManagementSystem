using Microsoft.AspNetCore.Mvc;
using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FleetManagementSystem.Controllers
{
    public class MileageController : Controller
    {
        private readonly FleetManagementDbContext _context;

        public MileageController(FleetManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var mileages = _context.Mileages.
                            Include(h => h.Vehicle).
            AsQueryable();

            var paginatedResult = await mileages.GetPagedAsync(page, pageSize);
            return View(paginatedResult);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchString, int page = 1, int pageSize = 10)
        {
            var mileages = _context.Mileages.
                            Include(h => h.Vehicle).
                            AsQueryable();


            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                mileages = mileages.Where(v => v.TotalMileage.ToLower().StartsWith(searchString) ||
                                                v.Vehicle.LicensePlate.ToLower().StartsWith(searchString)
                                                );
            }
            var paginatedResult = await mileages.GetPagedAsync(page, pageSize);

            return PartialView("_MileageTablePartial", paginatedResult);

            //return PartialView("_ServiceHistoryTablePartial", await histories.ToListAsync());
        }

        // Create GET
        public IActionResult Create()
        {
            ViewBag.VehicleList = new SelectList(_context.Vehicles, "Id", "LicensePlate");
            return View();
        }

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Mileage mileage, IFormFile proofFile)
        {
            if (ModelState.IsValid)
            {
                // Ensure that the VehicleId is assigned and corresponds to a valid vehicle
                if (mileage.VehicleId != null)
                {
                    var vehicle = await _context.Vehicles.FindAsync(mileage.VehicleId);
                    if (vehicle == null)
                    {
                        ModelState.AddModelError("VehicleId", "Invalid vehicle selected.");
                        ViewBag.VehicleList = new SelectList(_context.Vehicles, "Id", "LicensePlate", mileage.VehicleId);
                        return View(mileage); // Show error in view
                    }

                    mileage.Vehicle = vehicle;
                }
                // Handle proof file upload
                if (proofFile != null)
                {
                    // Ensure MOT directory exists
                    var mileageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProofFiles");
                    if (!Directory.Exists(mileageDirectory))
                    {
                        Directory.CreateDirectory(mileageDirectory);
                    }
                    var filePath = Path.Combine(mileageDirectory, proofFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await proofFile.CopyToAsync(stream);
                    }
                    mileage.ProofFilePath = $"/ProofFiles/{proofFile.FileName}";
                }

                _context.Add(mileage);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Vehicle", new { id = mileage.VehicleId });
            }
            // If the model state is invalid, re-populate the vehicle dropdown and return the form
            ViewBag.VehicleList = new SelectList(_context.Vehicles, "Id", "LicensePlate", mileage.VehicleId);
            return View(mileage);
        }

        // Edit GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mileage = await _context.Mileages.FindAsync(id);
            if (mileage == null)
            {
                return NotFound();
            }
            ViewBag.VehicleList = new SelectList(_context.Vehicles, "Id", "LicensePlate", mileage.VehicleId);
            return View(mileage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Mileage mileage, IFormFile proofFile)
        {
            if (id != mileage.Id)
            {
                return NotFound();
            }

            var existingMileage = await _context.Mileages.FindAsync(id);
            if (existingMileage == null)
            {
                return NotFound();
            }
            
            ModelState.Remove("proofFile");
            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Ensure ProofFiles directory exists
                    var proofDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProofFiles");
                    if (!Directory.Exists(proofDirectory))
                    {
                        Directory.CreateDirectory(proofDirectory);
                    }

                    // Handle proof file upload, only replace the file if a new one is uploaded
                    if (proofFile != null)
                    {
                        var filePath = Path.Combine(proofDirectory, proofFile.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await proofFile.CopyToAsync(stream);
                        }
                        mileage.ProofFilePath = $"/ProofFiles/{proofFile.FileName}"; // Update ProofFilePath if a new file is uploaded
                    }
                    else
                    {
                        // Keep the existing proof file if no new file is uploaded
                        mileage.ProofFilePath = existingMileage.ProofFilePath;
                    }

                    // Update the rest of the mileage properties
                    _context.Entry(existingMileage).CurrentValues.SetValues(mileage);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Mileages.Any(e => e.Id == mileage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Vehicle", new { id = mileage.VehicleId });
            }

            return View(mileage);
        }

        // Delete GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mileage = await _context.Mileages.Include(m => m.Vehicle).FirstOrDefaultAsync(m => m.Id == id);
            if (mileage == null)
            {
                return NotFound();
            }

            return View(mileage);
        }

        // Delete POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mileage = await _context.Mileages.FindAsync(id);
            _context.Mileages.Remove(mileage);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Vehicle", new { id = mileage.VehicleId });
        }
    }
}