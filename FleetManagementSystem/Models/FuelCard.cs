using System;
using System.ComponentModel.DataAnnotations;

namespace FleetManagementSystem.Models
{
    public class FuelCard
    {
        public int Id { get; set; }

        [Required]
        public string FuelCardNumber { get; set; }
        public int Mileage { get; set; }
        public int DriverId { get; set; }
        public Driver? Driver { get; set; }

        // List of fuel card usage details
        public List<FuelCardDetail>? FuelCardDetails { get; set; } = new List<FuelCardDetail>();  // Initialize it here
    }

    public class FuelCardDetail
    {
        public int Id { get; set; }
        public int FuelCardId { get; set; }
        public FuelCard? FuelCard { get; set; }
        public decimal Usage { get; set; } // in liters
        public int Month { get; set; }  // New field for the month
        public int Year { get; set; }   // New field for the year
        public string? ProofFilePath { get; set; } // Zip file path for receipts
        public string? Remarks { get; set; }
    }
}

