using System.Text.Json.Serialization;

namespace FleetManagementSystem.Models
{
    public class VehicleDashboardViewModel
    {
        public string VehicleReg { get; set; }
        public string VehicleMake { get; set; }
        public string DriverAssigned { get; set; }
        public double FuelUsedPerMonthByDriver { get; set; }
        public double MileagePerMonth { get; set; }
        public double ServicesCostPerMonth { get; set; }
        public double TotalCostPerMonth { get; set; }
    }

    public class DriverDashboardViewModel
    {
        public string DriverName { get; set; }
        public double FuelUsedPerMonth { get; set; }
        public double TotalSpentOnFines { get; set; }
    }
}