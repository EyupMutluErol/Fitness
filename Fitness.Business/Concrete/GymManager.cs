using Fitness.Business.Abstract;
using Fitness.Data.Abstract;
using Fitness.Entities.Concrete;

namespace Fitness.Business.Concrete;

public class GymManager : IGymService
{
    private readonly IGymRepository _gymRepository;

    public GymManager(IGymRepository gymRepository)
    {
        _gymRepository = gymRepository;
    }

    public async Task AddGymAsync(Gym gym)
    {
        var existingGym = await _gymRepository.GetAllAsync();
        if (existingGym.Any())
        {
            throw new InvalidOperationException("Sadece bir tane spor salonu eklenebilir.");
        }
        await _gymRepository.AddAsync(gym);
        await _gymRepository.SaveAsync();
    }
    public async Task UpdateGymAsync(Gym gym)
    {
        await _gymRepository.UpdateAsync(gym);
        await _gymRepository.SaveAsync();
    }

    public async Task DeleteGymAsync(int id)
    {
        var gymToDelete = await _gymRepository.GetByIdAsync(id);
        if (gymToDelete != null)
        {
            await _gymRepository.DeleteAsync(gymToDelete);
            await _gymRepository.SaveAsync();
        }
    }

    public async Task<List<Gym>> GetAllGymsAsync()
    {
        return await _gymRepository.GetAllAsync();
    }

    public async Task<Gym> GetGymDetailsAsync(int id)
    {
        return await _gymRepository.GetByIdAsync(id);
    }

    
}
