using Fitness.Entities.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fitness.Data.Concrete;

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

        modelBuilder.Entity<ServiceTrainer>()
            .HasKey(st => new { st.ServiceId, st.TrainerId });
    }
}
