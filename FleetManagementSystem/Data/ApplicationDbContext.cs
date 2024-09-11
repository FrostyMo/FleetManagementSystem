using FleetManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    //public DbSet<FleetManagementDbContext> FileMetadata { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}

//public class FleetManagementDbContext : DbContext
//{
//    public FleetManagementDbContext(DbContextOptions<FleetManagementDbContext> options)
//        : base(options)
//    {
//    }
//    public DbSet<Driver> Drivers { get; set; }
//    public DbSet<Vehicle> Vehicles { get; set; }
//    public DbSet<FuelCard> FuelCards { get; set; }
//    public DbSet<Fine> Fines { get; set; }
//    public DbSet<Insurance> Insurances { get; set; }
//    public DbSet<ServiceHistory> ServiceHistories { get; set; }
//    public DbSet<ReportIssue> ReportIssues { get; set; }
//    // New DbSet properties to handle fleet management
//    public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }
//    public DbSet<TripRecord> TripRecords { get; set; }
//}