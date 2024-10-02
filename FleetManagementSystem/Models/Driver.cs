namespace FleetManagementSystem.Models
{
    public class Driver
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime LicenseExpiry { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        // Navigation property for the assigned driver
        public ICollection<Fine>? Fines { get; set; } // Add this line
    }
}