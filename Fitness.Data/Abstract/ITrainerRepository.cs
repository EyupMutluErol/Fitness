using Fitness.Entities.Concrete;
namespace Fitness.Data.Abstract;
public interface ITrainerRepository:IGenericRepository<Trainer>
{
    Task<List<Trainer>> GetTrainersBySpecializationAsync(string specialization);
    Task<List<Trainer>> GetTrainersWithServiceAsync();
    Task<Trainer> GetTrainerWithDetailsAsync(int id);
}
