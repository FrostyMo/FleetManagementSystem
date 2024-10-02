using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FleetManagementSystem.Models
{
    public class Fine
    {
        public int Id { get; set; }

        [Required]
        public string FineReferenceNumber { get; set; }

        [Required]
        public DateTime DateIssued { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public bool IsPaid { get; set; }

        public string? ProofOfPaymentPath { get; set; }

        public int DriverId { get; set; }
        [JsonIgnore]
        public Driver? Driver { get; set; }
    }
}

