using Fitness.Data.Abstract;
using Fitness.Data.Concrete.Context;
using Fitness.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Fitness.Data.Concrete;

public class EfServiceRepository : GenericRepository<Service>, IServiceRepository
{
    public EfServiceRepository(FitnessDbContext context) : base(context)
    {
    }

    public async Task<List<Service>> GetServicesWithTrainersAsync()
    {
        return await _context.Set<Service>()
            .Include(s => s.ServiceTrainers)
            .ThenInclude(st => st.Trainer)
            .ToListAsync();
    }
}
