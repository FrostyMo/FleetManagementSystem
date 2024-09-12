using Microsoft.AspNetCore.Mvc;
using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FleetManagementSystem.Controllers
{
    public class VehicleController : Controller
    {
        private readonly FleetManagementDbContext _context;

        public VehicleController(FleetManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var vehicles = _context.Vehicles
                           .Include(v => v.Driver) // Eagerly load the Driver object
                           .AsQueryable();

            return View(await vehicles.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchString)
        {
            var vehicles = _context.Vehicles
                           .Include(v => v.Driver) // Eagerly load the Driver object
                           .AsQueryable();
            searchString = searchString.ToLower();
            if (!string.IsNullOrEmpty(searchString))
            {
                vehicles = vehicles.Where(v => v.LicensePlate.ToLower().StartsWith(searchString) ||
                                                v.Manufacturer.ToLower().StartsWith(searchString) ||
                                                v.Model.ToLower().StartsWith(searchString));
            }

            return PartialView("_VehicleTablePartial", await vehicles.ToListAsync());
        }

        //public IActionResult Create()
        //{
        //    return View();
        //}
        public async Task<IActionResult> Create()
        {
            ViewBag.DriverList = new SelectList(await _context.Drivers.ToListAsync(), "Id", "FirstName");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vehicle vehicle, IFormFile MOTFile)
        {
            if (ModelState.IsValid)
            {
                // Ensure MOT directory exists
                var motDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/MOTFiles");
                if (!Directory.Exists(motDirectory))
                {
                    Directory.CreateDirectory(motDirectory);
                }

                // Handle MOT file upload
                if (MOTFile != null)
                {
                    var filePath = Path.Combine(motDirectory, MOTFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await MOTFile.CopyToAsync(stream);
                    }
                    vehicle.MOTFilePath = $"/MOTFiles/{MOTFile.FileName}";
                }

                // DriverId is already being assigned through model binding, no need to re-assign Driver object.

                _context.Add(vehicle);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Populate Driver list in case of error
            ViewBag.DriverList = new SelectList(await _context.Drivers.ToListAsync(), "Id", "FirstName", vehicle.DriverId);
            return View(vehicle);
        }

        // GET: Vehicle/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                                .Include(v => v.Driver) // Eagerly load the Driver object
                                .Include(v => v.ServiceHistories) // Eagerly load the ServiceHistories
                                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            ViewBag.DriverList = new SelectList(await _context.Drivers.ToListAsync(), "Id", "FirstName", vehicle.DriverId);
            return View(vehicle);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vehicle vehicle, IFormFile MOTFile)
        {
            if (id != vehicle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Ensure MOT directory exists
                    var motDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/MOTFiles");
                    if (!Directory.Exists(motDirectory))
                    {
                        Directory.CreateDirectory(motDirectory);
                    }

                    // Handle MOT file upload, only replace the file if a new one is uploaded
                    if (MOTFile != null)
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/MOTFiles", MOTFile.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await MOTFile.CopyToAsync(stream);
                        }
                        vehicle.MOTFilePath = $"/MOTFiles/{MOTFile.FileName}";
                    }
                    else
                    {
                        // Do not overwrite MOTFilePath if no new file is uploaded
                        _context.Entry(vehicle).Property(v => v.MOTFilePath).IsModified = false;
                    }

                    // DriverId is already being assigned through model binding, no need to re-assign Driver object.

                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Populate Driver list in case of error
            ViewBag.DriverList = new SelectList(await _context.Drivers.ToListAsync(), "Id", "FirstName", vehicle.DriverId);
            return View(vehicle);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null) return NotFound();

            return View(vehicle);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Vehicles.Remove(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicles.Any(e => e.Id == id);
        }
    }
}