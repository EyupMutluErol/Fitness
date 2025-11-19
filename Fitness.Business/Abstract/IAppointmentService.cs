using Fitness.Entities.Concrete;

namespace Fitness.Business.Abstract;

public interface IAppointmentService
{
    Task<List<Appointment>> GetUserAppointments(string userId);
    Task<List<Appointment>> GetTrainerAppointments(int trainerId);
    Task<Appointment> GetAppointmentById(int id);
    Task<bool> CheckAvailabilityAndBook(Appointment appointment);
    Task<bool> ApproveAppointment(int appointmentId , string adminNotes);
    Task<bool> CancelAppointment(int appointmentId);

}
