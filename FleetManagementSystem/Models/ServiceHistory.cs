using System.Text.Json.Serialization;

namespace FleetManagementSystem.Models
{
    public class ServiceHistory
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } // Service Type, e.g., "Oil Change"
        public decimal Cost { get; set; }
        public string Status { get; set; } // Reimbursed or Pending
        public int VehicleId { get; set; } // Foreign key to Vehicle

        [JsonIgnore] // Prevent circular reference during serialization
        public Vehicle? Vehicle { get; set; } // Navigation property
    }
}