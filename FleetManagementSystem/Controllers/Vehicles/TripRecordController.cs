using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FleetManagementSystem.Controllers
{
    public class TripRecordController : Controller
    {
        private readonly FleetManagementDbContext _context;

        public TripRecordController(FleetManagementDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var tripRecords = _context.TripRecords.Include(t => t.Vehicle).Include(t => t.Driver).ToList();
            return View(tripRecords);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(TripRecord tripRecord)
        {
            if (ModelState.IsValid)
            {
                _context.TripRecords.Add(tripRecord);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tripRecord);
        }

        public IActionResult Edit(int id)
        {
            var tripRecord = _context.TripRecords.Find(id);
            return View(tripRecord);
        }

        [HttpPost]
        public IActionResult Edit(TripRecord tripRecord)
        {
            if (ModelState.IsValid)
            {
                _context.TripRecords.Update(tripRecord);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tripRecord);
        }

        public IActionResult Delete(int id)
        {
            var tripRecord = _context.TripRecords.Find(id);
            _context.TripRecords.Remove(tripRecord);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}