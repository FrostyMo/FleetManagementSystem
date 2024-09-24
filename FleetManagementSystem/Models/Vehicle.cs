namespace FleetManagementSystem.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string LicensePlate { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        // New Properties
        public string? MOTFilePath { get; set; } // Path to the MOT PDF file
        public bool IsTaxPaid { get; set; }     // Paid or Unpaid Tax
        public int? DriverId { get; set; }      // Assigned Driver ID (nullable)
        public string VIN { get; set; } // Vehicle Identification Number
        public DateTime RegistrationExpiry { get; set; }

        // Navigation property for the assigned driver
        public Driver? Driver { get; set; }

        // Navigation property for the assigned driver
        public ICollection<ServiceHistory>? ServiceHistories { get; set; } // Add this line

        public ICollection<Mileage> Mileages { get; set; } // Add this for Mileage tracking
    }
}