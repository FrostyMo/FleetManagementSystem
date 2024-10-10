using System;
namespace FleetManagementSystem.Models
{
	public class Insurance
	{
        public int Id { get; set; }

        // Insurance Details
        public string PolicyNumber { get; set; }
        public string Company { get; set; }
        public string Type { get; set; }   // Insurance Type: e.g., Motor, Liability
        public decimal CoverageAmount { get; set; }
        public decimal Premium { get; set; }

        // Validity dates
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // File upload for policy documents
        public string? PolicyDocuments { get; set; }

        // Remarks
        public string? Remarks { get; set; }

        // Foreign Keys to link to Drivers or Vehicles
        public List<Vehicle>? Vehicles { get; set; } // Optionally link to Vehicles (one insurance for multiple vehicles)
        public List<Driver>? Drivers { get; set; }   // Optionally link to Drivers (one insurance for multiple drivers)
    }
}

