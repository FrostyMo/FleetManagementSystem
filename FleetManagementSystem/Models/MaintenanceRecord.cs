using System;
namespace FleetManagementSystem.Models
{
    public class MaintenanceRecord
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } // Foreign key to Vehicle
        public DateTime ServiceDate { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public string ServiceProvider { get; set; }
    }
}

