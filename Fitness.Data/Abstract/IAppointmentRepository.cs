using Fitness.Entities.Concrete;

namespace Fitness.Data.Abstract;

public interface IAppointmentRepository:IGenericRepository<Appointment>
{
    Task<List<Appointment>> GetAppointmentByUserIdAsync(string userId);
    Task<List<Appointment>> GetAppointmentByTrainerIdAsync(int trainerId);
}
