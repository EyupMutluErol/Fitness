using Fitness.Entities.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fitness.Data.Concrete.Context;

public class FitnessDbContext:IdentityDbContext<AppUser,IdentityRole,string>
{
    public FitnessDbContext(DbContextOptions<FitnessDbContext> options):base(options)
    {
        
    }

    public DbSet<Gym> Gyms { get; set; }
    public DbSet<Trainer> Trainers { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Availability> Availabilities { get; set; }
    public DbSet<ServiceTrainer> ServiceTrainers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.AppUser) 
            .WithMany(u => u.Appointments) 
            .HasForeignKey(a => a.AppUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Trainer>()
            .HasOne(t => t.AppUser) 
            .WithOne(u => u.TrainerProfile) 
            .HasForeignKey<Trainer>(t => t.AppUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ServiceTrainer>()
            .HasKey(st => new { st.ServiceId, st.TrainerId });
    }
}
