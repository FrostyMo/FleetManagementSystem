using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FleetManagementSystem.Controllers
{
    public class MaintenanceRecordController : Controller
    {
        private readonly FleetManagementDbContext _context;

        public MaintenanceRecordController(FleetManagementDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var maintenanceRecords = _context.MaintenanceRecords.Include(m => m.Vehicle).ToList();
            return View(maintenanceRecords);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(MaintenanceRecord maintenanceRecord)
        {
            if (ModelState.IsValid)
            {
                _context.MaintenanceRecords.Add(maintenanceRecord);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(maintenanceRecord);
        }

        public IActionResult Edit(int id)
        {
            var maintenanceRecord = _context.MaintenanceRecords.Find(id);
            return View(maintenanceRecord);
        }

        [HttpPost]
        public IActionResult Edit(MaintenanceRecord maintenanceRecord)
        {
            if (ModelState.IsValid)
            {
                _context.MaintenanceRecords.Update(maintenanceRecord);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(maintenanceRecord);
        }

        public IActionResult Delete(int id)
        {
            var maintenanceRecord = _context.MaintenanceRecords.Find(id);
            _context.MaintenanceRecords.Remove(maintenanceRecord);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}