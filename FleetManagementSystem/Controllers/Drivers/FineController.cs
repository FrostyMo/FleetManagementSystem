using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FleetManagementSystem.Controllers
{
    public class FineController : Controller
    {
        private readonly FleetManagementDbContext _context;

        public FineController(FleetManagementDbContext context)
        {
            _context = context;
        }

        // Index (paginated)
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var fines = _context.Fines.Include(f => f.Driver).AsQueryable();
            var paginatedResult = await fines.GetPagedAsync(page, pageSize);
            paginatedResult.Action = "Index";
            return View(paginatedResult);
        }

        // Search function
        [HttpGet]
        public async Task<IActionResult> Search(string searchString, int page = 1, int pageSize = 10)
        {
            var fines = _context.Fines.Include(f => f.Driver).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                fines = fines.Where(f => f.FineReferenceNumber.ToLower().Contains(searchString) ||
                                         f.Driver.FirstName.ToLower().Contains(searchString) ||
                                         f.Driver.LastName.ToLower().Contains(searchString));
            }

            var paginatedResult = await fines.GetPagedAsync(page, pageSize);
            paginatedResult.Action = "Index";
            return PartialView("_FineTablePartial", paginatedResult);
        }

        // Create GET
        public IActionResult Create()
        {
            ViewBag.DriverList = _context.Drivers.Select(d => new SelectListItem
                                        {
                                            Value = d.Id.ToString(),
                                            Text = $"{d.FirstName} {d.LastName}"
                                        })
                                        .ToList();
            return View();
        }

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Fine fine, IFormFile proofFile)
        {
            ModelState.Remove("proofFile");
            if (ModelState.IsValid)
            {
                if (proofFile != null)
                {
                    var proofDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProofOfPayments");
                    if (!Directory.Exists(proofDirectory))
                    {
                        Directory.CreateDirectory(proofDirectory);
                    }
                    var filePath = Path.Combine(proofDirectory, proofFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await proofFile.CopyToAsync(stream);
                    }
                    fine.ProofOfPaymentPath = $"/ProofOfPayments/{proofFile.FileName}";
                }

                _context.Add(fine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DriverList = _context.Drivers.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"{d.FirstName} {d.LastName}"
            })
                                        .ToList();
            return View(fine);
        }

        // Edit GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var fine = await _context.Fines.FindAsync(id);
            if (fine == null) return NotFound();

            ViewBag.DriverList = _context.Drivers.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"{d.FirstName} {d.LastName}"
            })
                                        .ToList();
            return View(fine);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Fine fine, IFormFile? proofFile)
        {
            if (id != fine.Id)
            {
                return NotFound();
            }
            var existingFine = await _context.Fines.FindAsync(id);
            if (existingFine == null)
            {
                return NotFound();
            }

            //ModelState.Remove("proofFile");
            if (ModelState.IsValid)
            {
                try
                {
                    //var existingFine = await _context.Fines.FindAsync(id);

                    if (proofFile != null)
                    {
                        var proofDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProofOfPayments");
                        if (!Directory.Exists(proofDirectory))
                        {
                            Directory.CreateDirectory(proofDirectory);
                        }
                        var filePath = Path.Combine(proofDirectory, proofFile.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await proofFile.CopyToAsync(stream);
                        }
                        fine.ProofOfPaymentPath = $"/ProofOfPayments/{proofFile.FileName}";
                    }
                    else
                    {
                        // Keep the existing proof file path if no new one is uploaded
                        fine.ProofOfPaymentPath = existingFine.ProofOfPaymentPath;
                    }

                    // Update the rest of the fine details
                    _context.Entry(existingFine).CurrentValues.SetValues(fine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Fines.Any(e => e.Id == fine.Id))
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

            ViewBag.DriverList = _context.Drivers.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"{d.FirstName} {d.LastName}"
            })
                                        .ToList();
            return View(fine);
        }

        //// Delete GET
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null) return NotFound();

        //    var fine = await _context.Fines.Include(f => f.Driver).FirstOrDefaultAsync(m => m.Id == id);
        //    if (fine == null) return NotFound();

        //    return View(fine);
        //}

        //// Delete POST
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var fine = await _context.Fines.FindAsync(id);
        //    _context.Fines.Remove(fine);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        // Delete GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var fine = await _context.Fines.Include(f => f.Driver).FirstOrDefaultAsync(f => f.Id == id);
            if (fine == null) return NotFound();

            return View(fine);
        }

        // Delete POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fine = await _context.Fines.FindAsync(id);
            if (fine == null) return NotFound();

            _context.Fines.Remove(fine);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}