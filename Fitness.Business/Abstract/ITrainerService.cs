using Fitness.Entities.Concrete;

namespace Fitness.Business.Abstract;

public interface ITrainerService
{
    Task AddTrainer(Trainer trainer);
    Task UpdateTrainer(Trainer trainer);
    Task DeleteTrainer(int id);

    Task<List<Trainer>> GetTrainersWithServices();
    Task<Trainer> GetTrainerDetails(int id);
    Task<List<Trainer>> GetAvailableTrainers(DateTime date , int serviceId);
}
