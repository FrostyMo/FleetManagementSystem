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

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var vehicles = _context.Vehicles
                           .Include(v => v.Driver) // Eagerly load the Driver object
                           .AsQueryable();
            var paginatedResult = await vehicles.GetPagedAsync(page, pageSize);
            paginatedResult.Action = "Index";
            //return View(await vehicles.ToListAsync());
            return View(paginatedResult);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchString, int page = 1, int pageSize = 10)
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
            var paginatedResult = await vehicles.GetPagedAsync(page, pageSize);
            paginatedResult.Action = "Index";
            return PartialView("_VehicleTablePartial", paginatedResult);

            //return PartialView("_VehicleTablePartial", await vehicles.ToListAsync());
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

        public async Task<IActionResult> Details(int? id, int serviceHistoryPage = 1, int mileagePage = 1, int pageSize = 3)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                                       .Include(v => v.Driver) // Eagerly load the Driver object
                                       .FirstOrDefaultAsync(m => m.Id == id);

            if (vehicle == null)
            {
                return NotFound();
            }

            // Fetch the related service histories for this vehicle and apply pagination
            var serviceHistoriesQuery = _context.ServiceHistories
                                                .Where(sh => sh.VehicleId == id)
                                                .OrderBy(sh => sh.Date); // Order by date

            var paginatedHistories = await serviceHistoriesQuery.GetPagedAsync(serviceHistoryPage, pageSize); // PagedResult for Service Histories
            paginatedHistories.Action = "Details";
            //paginatedHistories.PageParameter = "serviceHistoryPage"; // Set the custom page parameter

            // Fetch the related mileages for this vehicle and apply pagination
            var mileagesQuery = _context.Mileages
                                        .Where(m => m.VehicleId == id)
                                        .OrderBy(m => m.Date); // Order by date

            var paginatedMileages = await mileagesQuery.GetPagedAsync(mileagePage, pageSize); // PagedResult for Mileages
            paginatedMileages.Action = "Details";
            //paginatedMileages.PageParameter = "mileagePage"; // Set the custom page parameter

            // Store the pagination results in ViewBag
            ViewBag.ServiceHistories = paginatedHistories;
            ViewBag.Mileages = paginatedMileages;

            // Return the vehicle along with the ViewBag pagination data
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
        public async Task<IActionResult> Edit(int id, Vehicle vehicle, IFormFile? MOTFile)
        {
            if (id != vehicle.Id)
            {
                return NotFound();
            }
            var existingVehicle = await _context.Vehicles.FindAsync(id);
            if (existingVehicle == null)
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
                        vehicle.MOTFilePath = existingVehicle.MOTFilePath;
                    }

                    // DriverId is already being assigned through model binding, no need to re-assign Driver object.
                    // Update the rest of the fine details
                    _context.Entry(existingVehicle).CurrentValues.SetValues(vehicle);
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
            // Check if any service histories or mileages are associated with the vehicle
            var hasServiceHistories = await _context.ServiceHistories.AnyAsync(sh => sh.VehicleId == vehicle.Id);
            var hasMileages = await _context.Mileages.AnyAsync(m => m.VehicleId == vehicle.Id);

            if (hasServiceHistories || hasMileages)
            {
                ViewBag.ErrorMessage = "This vehicle has associated service histories or mileages. Delete them before deleting the vehicle.";
            }
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


        [HttpGet]
        public async Task<IActionResult> ServiceHistoryPage(int vehicleId, int serviceHistoryPage = 1, int pageSize = 3)
        {
            var serviceHistoriesQuery = _context.ServiceHistories
                                                .Where(sh => sh.VehicleId == vehicleId)
                                                .OrderBy(sh => sh.Date)
                                                .AsQueryable();

            var paginatedHistories = await serviceHistoriesQuery.GetPagedAsync(serviceHistoryPage, pageSize);
            ViewBag.VehicleId = vehicleId; // Pass vehicle ID to route the pagination correctly

            return PartialView("_DetailsPartialServiceHistory", paginatedHistories);
        }

        [HttpGet]
        public async Task<IActionResult> MileagePage(int vehicleId, int mileagePage = 1, int pageSize = 3)
        {
            var mileagesQuery = _context.Mileages
                                        .Where(m => m.VehicleId == vehicleId)
                                        .OrderBy(m => m.Date)
                                        .AsQueryable();

            var paginatedMileages = await mileagesQuery.GetPagedAsync(mileagePage, pageSize);
            ViewBag.VehicleId = vehicleId; // Pass vehicle ID to route the pagination correctly

            return PartialView("_DetailsPartialMileage", paginatedMileages);
        }

    }
}