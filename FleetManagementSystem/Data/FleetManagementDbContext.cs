﻿using FleetManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetManagementSystem.Data
{
    public class FleetManagementDbContext : DbContext
    {
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        
        public DbSet<Fine> Fines { get; set; }
        
        public DbSet<ServiceHistory> ServiceHistories { get; set; }
        public DbSet<Mileage> Mileages { get; set; }

        public DbSet<FuelCard> FuelCards { get; set; }
        public DbSet<FuelCardDetail> FuelCardDetails { get; set; }

        public DbSet<Insurance> Insurances { get; set; }
        public FleetManagementDbContext(DbContextOptions<FleetManagementDbContext> options)
            : base(options)
        {
        }
    }
}

