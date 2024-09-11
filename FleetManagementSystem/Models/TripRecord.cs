using System;
namespace FleetManagementSystem.Models
{
    public class TripRecord
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } // Foreign key to Vehicle
        public int DriverId { get; set; }
        public Driver Driver { get; set; } // Foreign key to Driver
        public DateTime TripStartDate { get; set; }
        public DateTime TripEndDate { get; set; }
        public int Mileage { get; set; }
        public string Notes { get; set; }
    }
}

