using Fitness.Data.Abstract;
using Fitness.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Fitness.Data.Concrete;

public class EfAvailabilityRepository : GenericRepository<Availability>, IAvailabilityRepository
{
    public EfAvailabilityRepository(FitnessDbContext context) : base(context)
    {
    }
}
