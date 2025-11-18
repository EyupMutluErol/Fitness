using Fitness.Entities.Concrete;

namespace Fitness.Data.Abstract;

public interface IServiceRepository:IGenericRepository<Service>
{
    Task<List<Service>> GetServicesWithTrainersAsync();
}
