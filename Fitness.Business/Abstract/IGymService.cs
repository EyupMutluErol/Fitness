using Fitness.Entities.Concrete;

namespace Fitness.Business.Abstract;

public interface IGymService
{
    Task<Gym> GetGymDetailsAsync(int id);
    Task<List<Gym>> GetAllGymsAsync();
    Task AddGymAsync(Gym gym);
    Task UpdateGymAsync(Gym gym);
    Task DeleteGymAsync(int id);
}
