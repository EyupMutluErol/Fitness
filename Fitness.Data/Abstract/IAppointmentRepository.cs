using Fitness.Entities.Concrete;

namespace Fitness.Data.Abstract;

public interface IAppointmentRepository:IGenericRepository<Appointment>
{
    Task<List<Appointment>> GetAppointmentsByUserIdAsync(string userId);
    Task<List<Appointment>> GetAppointmentsByTrainerIdAsync(int trainerId);
}
