using Microsoft.AspNetCore.Mvc;
using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace FleetManagementSystem.Controllers
{
    public class FuelCardDetailController : Controller
    {
        private readonly FleetManagementDbContext _context;

        public FuelCardDetailController(FleetManagementDbContext context)
        {
            _context = context;

        }
        [HttpGet]
        public async Task<IActionResult> FuelCardDetails(int fuelCardId, int page = 1, int pageSize = 10)
        {
            var detailsQuery = _context.FuelCardDetails
                                       .Where(fcd => fcd.FuelCardId == fuelCardId)
                                       .OrderBy(fcd => fcd.Year)
                                       .ThenByDescending(fcd => fcd.Month)
                                       .AsQueryable();
            

            var paginatedDetails = await detailsQuery.GetPagedAsync(page, pageSize); // Pagination logic here
            return PartialView("_FuelCardDetailsPartial", paginatedDetails);
        }

        // GET: Create Detail
        [HttpGet]
        public IActionResult Create(int fuelCardId)
        {
            var fuelCard = _context.FuelCards.Find(fuelCardId);
            if (fuelCard == null) return NotFound();

            var model = new FuelCardDetail
            {
                FuelCardId = fuelCardId,
            };
            return View(model);
        }

        // POST: Create Detail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FuelCardDetail fuelCardDetail, IFormFile proofFile)
        {
            if (ModelState.IsValid)
            {
                if (proofFile != null)
                {
                    var proofDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/FuelCardProofs");
                    if (!Directory.Exists(proofDirectory))
                    {
                        Directory.CreateDirectory(proofDirectory);
                    }

                    var filePath = Path.Combine(proofDirectory, proofFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await proofFile.CopyToAsync(stream);
                    }

                    fuelCardDetail.ProofFilePath = $"/FuelCardProofs/{proofFile.FileName}";
                }

                _context.Add(fuelCardDetail);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "FuelCard");
            }
            return View(fuelCardDetail);
        }

        // GET: Edit Detail
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var fuelCardDetail = _context.FuelCardDetails
                                 .Include(f => f.FuelCard)
                                 .FirstOrDefault(f => f.Id == id);
            if (fuelCardDetail == null) return NotFound();

            return View(fuelCardDetail);
        }

        // POST: Edit Detail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FuelCardDetail fuelCardDetail, IFormFile? proofFile)
        {
            if (id != fuelCardDetail.Id)
            {
                return NotFound();
            }
            var existingDetail = await _context.FuelCardDetails
                                       .Include(f => f.FuelCard)
                                       .FirstOrDefaultAsync(f => f.Id == id);
            if (existingDetail == null)
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
                        var proofDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/FuelCardProofs");
                        if (!Directory.Exists(proofDirectory))
                        {
                            Directory.CreateDirectory(proofDirectory);
                        }
                        var filePath = Path.Combine(proofDirectory, proofFile.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await proofFile.CopyToAsync(stream);
                        }
                        fuelCardDetail.ProofFilePath = $"/FuelCardProofs/{proofFile.FileName}";
                    }
                    else
                    {
                        // Keep the existing proof file path if no new one is uploaded
                        fuelCardDetail.ProofFilePath = existingDetail.ProofFilePath;
                    }

                    // Update the rest of the fine details
                    _context.Entry(existingDetail).CurrentValues.SetValues(fuelCardDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.FuelCardDetails.Any(e => e.Id == fuelCardDetail.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "FuelCard", new { id = existingDetail.FuelCardId });
            }
            return View(fuelCardDetail);
        }

        // GET: Delete Detail
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var fuelCardDetail = _context.FuelCardDetails
                .Include(f => f.FuelCard)
                .FirstOrDefault(f => f.Id == id);
            if (fuelCardDetail == null) return NotFound();

            return View(fuelCardDetail);
        }

        // POST: Delete Detail
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fuelCardDetail = await _context.FuelCardDetails.FindAsync(id);
            if (fuelCardDetail == null) return NotFound();

            _context.FuelCardDetails.Remove(fuelCardDetail);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "FuelCard");
        }


        Dictionary<string, string> months = new Dictionary<string, string>()
        {
            { "january", "01"},
            { "february", "02"},
            { "march", "03"},
            { "april", "04"},
            { "may", "05"},
            { "june", "06"},
            { "july", "07"},
            { "august", "08"},
            { "september", "09"},
            { "october", "10"},
            { "november", "11"},
            { "december", "12"},
        };
    }
}