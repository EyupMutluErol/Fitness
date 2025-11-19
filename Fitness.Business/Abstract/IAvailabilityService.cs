using Fitness.Entities.Concrete;

namespace Fitness.Business.Abstract;

public interface IAvailabilityService
{
    Task<List<Availability>> GetTrainerAvailabilityAsync(int trainerId);
    Task<bool> AddAvailabilityBlockAsync(Availability availability);
    Task<bool> RemoveAvailabilityBlockAsync(int availabilityId);
}
