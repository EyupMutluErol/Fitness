using Fitness.Data.Abstract;
using Fitness.Data.Concrete.Context;
using Fitness.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Fitness.Data.Concrete;

public class EfTrainerRepository : GenericRepository<Trainer>, ITrainerRepository
{
    public EfTrainerRepository(FitnessDbContext context) : base(context)
    {
    }

    public async Task<List<Trainer>> GetTrainersBySpecializationAsync(string specialization)
    {
        return await _context.Set<Trainer>()
                .Where(t => t.Specialization.Contains(specialization)) 
                .ToListAsync();
    }

    public async Task<List<Trainer>> GetTrainersWithServiceAsync()
    {
        return await _context.Set<Trainer>()
            .Include(t => t.ServiceTrainers)
            .ThenInclude(st => st.Service)
            .ToListAsync();
    }

    public async Task<Trainer> GetTrainerWithDetailsAsync(int id)
    {
        return await _context.Set<Trainer>()
            .Include(t => t.Availabilities)
            .Include(t => t.ServiceTrainers)
            .ThenInclude(st => st.Service)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}
