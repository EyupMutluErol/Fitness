using Fitness.Entities.Concrete;

namespace Fitness.Business.Abstract;

public interface IServiceService
{
    Task<List<Service>> GetAllServicesAsync();
    Task<List<Service>> GetServicesWithTrainersAsync();
    Task<Service> GetServiceByIdAsync(int id);
    Task AddServiceAsync(Service service);
    Task UpdateServiceAsync(Service service);
    Task DeleteServiceAsync(int id);
}
