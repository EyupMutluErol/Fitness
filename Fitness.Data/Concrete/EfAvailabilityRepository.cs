using Fitness.Data.Abstract;
using Fitness.Data.Concrete.Context;
using Fitness.Entities.Concrete;


namespace Fitness.Data.Concrete;

public class EfAvailabilityRepository : GenericRepository<Availability>, IAvailabilityRepository
{
    public EfAvailabilityRepository(FitnessDbContext context) : base(context)
    {
    }
}
