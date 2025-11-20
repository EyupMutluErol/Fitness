using Fitness.Data.Abstract;
using Fitness.Data.Concrete.Context;
using Fitness.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Fitness.Data.Concrete;

public class EfAppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
{
    public EfAppointmentRepository(FitnessDbContext context) : base(context)
    {
    }

    public async Task<List<Appointment>> GetAppointmentsByTrainerIdAsync(int trainerId)
    {
        return await _context.Set<Appointment>()
            .Include(a=>a.AppUser)
            .Include(a=>a.Service)
            .Where(a=>a.TrainerId==trainerId)
            .OrderByDescending(a=>a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<List<Appointment>> GetAppointmentsByUserIdAsync(string userId)
    {
         return await _context.Set<Appointment>()
            .Include(a=>a.Trainer)
            .Include(a=>a.Service)
            .Where(a => a.AppUserId == userId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();
    }
}
