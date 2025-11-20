using Fitness.Data.Abstract;
using Fitness.Data.Concrete.Context;
using Fitness.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Fitness.Data.Concrete;

public class EfGymRepository : GenericRepository<Gym>, IGymRepository
{
    public EfGymRepository(FitnessDbContext context) : base(context)
    {
    }
}
