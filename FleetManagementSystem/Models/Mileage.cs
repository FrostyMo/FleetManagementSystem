using System.Text.Json.Serialization;

namespace FleetManagementSystem.Models
{
    public class Mileage
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double? TotalMileage { get; set; }
        public string? ProofFilePath { get; set; } // Path to the uploaded image or missing text
        public int VehicleId { get; set; }
        [JsonIgnore]
        public Vehicle? Vehicle { get; set; } // Navigation property to Vehicle
    }
    
}