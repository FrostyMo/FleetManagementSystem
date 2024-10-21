using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FleetManagementSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly FleetManagementDbContext _context;

        public DashboardController(FleetManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string vehicleFilterType = "month", int? vehicleYear = null, int? vehicleMonth = null,
            string driverFilterType = "month", int? driverYear = null, int? driverMonth = null,
            int vehiclePage = 1, int vehiclePageSize = 1,
            int driverPage = 1, int driverPageSize = 2)
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            // Vehicle Filter Defaults
            var selectedVehicleYear = vehicleYear ?? currentYear;
            var selectedVehicleMonth = vehicleMonth ?? currentMonth;

            // Driver Filter Defaults
            var selectedDriverYear = driverYear ?? currentYear;
            var selectedDriverMonth = driverMonth ?? currentMonth;

            ViewBag.VehicleFilterType = vehicleFilterType;
            ViewBag.DriverFilterType = driverFilterType;

            // Vehicle Dashboard Filtering
            IQueryable<VehicleDashboardViewModel> vehicleDashboardQuery = _context.Vehicles
                .Select(v => new VehicleDashboardViewModel
                {
                    VehicleReg = v.LicensePlate,
                    VehicleMake = v.Manufacturer + " " + v.Model,
                    DriverAssigned = v.Driver != null ? v.Driver.FirstName + " " + v.Driver.LastName : "Unassigned",
                    FuelUsedPerMonthByDriver = _context.FuelCardDetails
                        .Where(f => f.FuelCard.DriverId == v.DriverId && f.Year == selectedVehicleYear && f.Month == selectedVehicleMonth)
                        .Sum(f => (double?)f.Usage) ?? 0,
                    MileagePerMonth = _context.Mileages
                        .Where(m => m.VehicleId == v.Id && m.Date.Year == selectedVehicleYear && m.Date.Month == selectedVehicleMonth)
                        .Sum(m => m.TotalMileage) ?? 0,
                    ServicesCostPerMonth = _context.ServiceHistories
                        .Where(s => s.VehicleId == v.Id && s.Date.Year == selectedVehicleYear && s.Date.Month == selectedVehicleMonth)
                        .Sum(s => (double?)s.Cost) ?? 0,
                    TotalCostPerMonth = (
                        _context.FuelCardDetails
                            .Where(f => f.FuelCard.DriverId == v.DriverId && f.Year == selectedVehicleYear && f.Month == selectedVehicleMonth)
                            .Sum(f => (double?)f.Usage) ?? 0) +
                            (_context.ServiceHistories
                            .Where(s => s.VehicleId == v.Id && s.Date.Year == selectedVehicleYear && s.Date.Month == selectedVehicleMonth)
                            .Sum(s => (double?)s.Cost) ?? 0)
                });

            // Apply Year Filter if applicable
            if (vehicleFilterType == "year")
            {
                vehicleDashboardQuery = _context.Vehicles
                    .Select(v => new VehicleDashboardViewModel
                    {
                        VehicleReg = v.LicensePlate,
                        VehicleMake = v.Manufacturer + " " + v.Model,
                        DriverAssigned = v.Driver != null ? v.Driver.FirstName + " " + v.Driver.LastName : "Unassigned",
                        FuelUsedPerMonthByDriver = _context.FuelCardDetails
                            .Where(f => f.FuelCard.DriverId == v.DriverId && f.Year == selectedVehicleYear)
                            .Sum(f => (double?)f.Usage) ?? 0,
                        MileagePerMonth = _context.Mileages
                            .Where(m => m.VehicleId == v.Id && m.Date.Year == selectedVehicleYear)
                            .Sum(m => m.TotalMileage) ?? 0,
                        ServicesCostPerMonth = _context.ServiceHistories
                            .Where(s => s.VehicleId == v.Id && s.Date.Year == selectedVehicleYear)
                            .Sum(s => (double?)s.Cost) ?? 0,
                        TotalCostPerMonth = (
                            _context.FuelCardDetails
                                .Where(f => f.FuelCard.DriverId == v.DriverId && f.Year == selectedVehicleYear)
                                .Sum(f => (double?)f.Usage) ?? 0) +
                                (_context.ServiceHistories
                                .Where(s => s.VehicleId == v.Id && s.Date.Year == selectedVehicleYear)
                                .Sum(s => (double?)s.Cost) ?? 0)
                    });
            }

            // Pagination for Vehicle Dashboard
            var vehicleTotalItems = await vehicleDashboardQuery.CountAsync();
            var vehicleTotalPages = (int)Math.Ceiling((double)vehicleTotalItems / vehiclePageSize);
            var vehicleDashboard = await vehicleDashboardQuery
                .Skip((vehiclePage - 1) * vehiclePageSize)
                .Take(vehiclePageSize)
                .ToListAsync();

            // Driver Dashboard Filtering
            IQueryable<DriverDashboardViewModel> driverDashboardQuery = _context.Drivers
                .Select(d => new DriverDashboardViewModel
                {
                    DriverName = d.FirstName + " " + d.LastName,
                    FuelUsedPerMonth = _context.FuelCardDetails
                        .Where(f => f.FuelCard.DriverId == d.Id && f.Year == selectedDriverYear && f.Month == selectedDriverMonth)
                        .Sum(f => (double?)f.Usage) ?? 0,
                    TotalSpentOnFines = _context.Fines
                        .Where(f => f.DriverId == d.Id && f.DateIssued.Year == selectedDriverYear && f.DateIssued.Month == selectedDriverMonth)
                        .Sum(f => (double?)f.Amount) ?? 0
                });

            // Apply Year Filter if applicable
            if (driverFilterType == "year")
            {
                driverDashboardQuery = _context.Drivers
                    .Select(d => new DriverDashboardViewModel
                    {
                        DriverName = d.FirstName + " " + d.LastName,
                        FuelUsedPerMonth = _context.FuelCardDetails
                            .Where(f => f.FuelCard.DriverId == d.Id && f.Year == selectedDriverYear)
                            .Sum(f => (double?)f.Usage) ?? 0,
                        TotalSpentOnFines = _context.Fines
                            .Where(f => f.DriverId == d.Id && f.DateIssued.Year == selectedDriverYear)
                            .Sum(f => (double?)f.Amount) ?? 0
                    });
            }

            // Pagination for Driver Dashboard
            var driverTotalItems = await driverDashboardQuery.CountAsync();
            var driverTotalPages = (int)Math.Ceiling((double)driverTotalItems / driverPageSize);
            var driverDashboard = await driverDashboardQuery
                .Skip((driverPage - 1) * driverPageSize)
                .Take(driverPageSize)
                .ToListAsync();

            // Calculate Total Fines across all drivers
            var totalFines = await _context.Fines.SumAsync(f => (double?)f.Amount) ?? 0;
            // Calculate Total Fuel Usage across all vehicles
            var totalFuelUsage = await _context.FuelCardDetails.SumAsync(f => (double?)f.Usage) ?? 0;


            // Summary Cards (Top 3)
            var topDriversWithFines = await _context.Drivers
                .Select(d => new
                {
                    DriverName = d.FirstName + " " + d.LastName,
                    TotalFines = _context.Fines
                                .Where(f => f.DriverId == d.Id)
                                .Sum(f => (double?)f.Amount) ?? 0
                })
                .OrderByDescending(d => d.TotalFines)
                .Take(3)
                .ToListAsync();

            var topVehiclesByFuelUsage = await _context.Drivers
                .Select(d => new
                {
                    Driver = d.FirstName + " " + d.LastName,
                    FuelUsed = _context.FuelCardDetails
                                .Where(f => f.FuelCard.DriverId == d.Id)
                                .Sum(f => (double?)f.Usage) ?? 0
                })
                .OrderByDescending(d => d.FuelUsed)
                .Take(3)
                .ToListAsync();

            // Query to find insurance records expiring within the next 30 days
            var today = DateTime.Now;
            var upcomingInsuranceExpirations = await _context.Insurances
                .Where(i => i.EndDate >= today && i.EndDate <= today.AddDays(30))
                .Select(i => new
                {
                    Policy     = i.PolicyNumber != null ? i.PolicyNumber : "Policy Number not found",
                    ExpiryDate = i.EndDate
                })
                .ToListAsync();

            
            

            // ViewBag Data for View
            ViewBag.VehicleDashboard = vehicleDashboard;
            ViewBag.DriverDashboard = driverDashboard;

            ViewBag.TotalFines = totalFines;
            ViewBag.TopDriversWithFines = topDriversWithFines;

            ViewBag.TotalFuelUsage = totalFuelUsage;
            ViewBag.TopVehiclesByFuelUsage = topVehiclesByFuelUsage;

            ViewBag.InsurancesExpiring = upcomingInsuranceExpirations.Count;
            ViewBag.UpcomingInsurances = upcomingInsuranceExpirations;

            // Pagination Info for Vehicles and Drivers
            ViewBag.VehicleCurrentPage = vehiclePage;
            ViewBag.VehicleTotalPages = vehicleTotalPages;

            ViewBag.DriverCurrentPage = driverPage;
            ViewBag.DriverTotalPages = driverTotalPages;

            return View();
        }


        // New method for AJAX loading of vehicle dashboard
        public async Task<IActionResult> LoadVehicleDashboard(string filterType = "month", int? year = null, int? month = null, int page = 1, int vehiclePageSize = 1)
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            var selectedYear = year ?? currentYear;
            var selectedMonth = month ?? currentMonth;

            IQueryable<VehicleDashboardViewModel> vehicleDashboardQuery = _context.Vehicles
                .Select(v => new VehicleDashboardViewModel
                {
                    VehicleReg = v.LicensePlate,
                    VehicleMake = v.Manufacturer + " " + v.Model,
                    DriverAssigned = v.Driver != null ? v.Driver.FirstName + " " + v.Driver.LastName : "Unassigned",
                    FuelUsedPerMonthByDriver = _context.FuelCardDetails
                        .Where(f => f.FuelCard.DriverId == v.DriverId && f.Year == selectedYear &&
                                    (filterType == "month" ? f.Month == selectedMonth : true))
                        .Sum(f => (double?)f.Usage) ?? 0,
                    MileagePerMonth = _context.Mileages
                        .Where(m => m.VehicleId == v.Id && m.Date.Year == selectedYear &&
                                    (filterType == "month" ? m.Date.Month == selectedMonth : true))
                        .Sum(m => m.TotalMileage) ?? 0,
                    ServicesCostPerMonth = _context.ServiceHistories
                        .Where(s => s.VehicleId == v.Id && s.Date.Year == selectedYear &&
                                    (filterType == "month" ? s.Date.Month == selectedMonth : true))
                        .Sum(s => (double?)s.Cost) ?? 0,
                    TotalCostPerMonth = (
                        _context.FuelCardDetails
                            .Where(f => f.FuelCard.DriverId == v.DriverId && f.Year == selectedYear &&
                                        (filterType == "month" ? f.Month == selectedMonth : true))
                            .Sum(f => (double?)f.Usage) ?? 0) +
                        (_context.ServiceHistories
                            .Where(s => s.VehicleId == v.Id && s.Date.Year == selectedYear &&
                                        (filterType == "month" ? s.Date.Month == selectedMonth : true))
                            .Sum(s => (double?)s.Cost) ?? 0)
                });

            // Pagination logic
            var vehicleTotalItems = await vehicleDashboardQuery.CountAsync();
            var vehicleTotalPages = (int)Math.Ceiling((double)vehicleTotalItems / vehiclePageSize);
            var vehicleDashboard = await vehicleDashboardQuery
                .Skip((page - 1) * vehiclePageSize)
                .Take(vehiclePageSize)
                .ToListAsync();

            ViewBag.VehicleCurrentPage = page;
            ViewBag.VehicleTotalPages = vehicleTotalPages;

            return PartialView("_VehicleDashboard", vehicleDashboard);
        }



        // New method for AJAX loading of driver dashboard
        public async Task<IActionResult> LoadDriverDashboard(string filterType = "month", int? year = null, int? month = null, int page = 1, int driverPageSize = 2)
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            var selectedYear = year ?? currentYear;
            var selectedMonth = month ?? currentMonth;

            IQueryable<DriverDashboardViewModel> driverDashboardQuery = _context.Drivers
                .Select(d => new DriverDashboardViewModel
                {
                    DriverName = d.FirstName + " " + d.LastName,
                    FuelUsedPerMonth = _context.FuelCardDetails
                        .Where(f => f.FuelCard.DriverId == d.Id && f.Year == selectedYear &&
                                    (filterType == "month" ? f.Month == selectedMonth : true)) // Handle monthly vs yearly
                        .Sum(f => (double?)f.Usage) ?? 0,
                    TotalSpentOnFines = _context.Fines
                        .Where(f => f.DriverId == d.Id && f.DateIssued.Year == selectedYear &&
                                    (filterType == "month" ? f.DateIssued.Month == selectedMonth : true)) // Handle monthly vs yearly
                        .Sum(f => (double?)f.Amount) ?? 0
                });

            // Apply pagination logic
            var driverTotalItems = await driverDashboardQuery.CountAsync();
            var driverTotalPages = (int)Math.Ceiling((double)driverTotalItems / driverPageSize);
            var driverDashboard = await driverDashboardQuery
                .Skip((page - 1) * driverPageSize)
                .Take(driverPageSize)
                .ToListAsync();

            ViewBag.DriverCurrentPage = page;
            ViewBag.DriverTotalPages = driverTotalPages;

            return PartialView("_DriverDashboard", driverDashboard);
        }


    }
}
