using Fitness.Business.Abstract;
using Fitness.Data.Abstract;
using Fitness.Entities.Concrete;

namespace Fitness.Business.Concrete;

public class AppointmentManager : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IServiceService _serviceService;
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly ITrainerRepository _trainerRepository;

    public AppointmentManager(IAppointmentRepository appointmentRepository, IServiceService serviceService, IAvailabilityRepository availabilityRepository,ITrainerRepository trainerRepository)
    {
        _appointmentRepository = appointmentRepository;
        _serviceService = serviceService;
        _availabilityRepository = availabilityRepository;
        _trainerRepository = trainerRepository;
    }

    public async Task<List<Appointment>> GetUserAppointments(string userId)
    {
        return await _appointmentRepository.GetAppointmentsByUserIdAsync(userId);
    }
    public async Task<List<Appointment>> GetTrainerAppointments(int trainerId)
    {
       return await _appointmentRepository.GetAppointmentsByTrainerIdAsync(trainerId);
    }
    public async Task<Appointment> GetAppointmentById(int id)
    {
        return await _appointmentRepository.GetByIdAsync(id);
    }
    public async Task<bool> CheckAvailabilityAndBook(Appointment appointment)
    {
        var trainer = await _trainerRepository.GetByIdAsync(appointment.TrainerId);
        if(trainer != null && trainer.AppUserId == appointment.AppUserId)
        {
            throw new InvalidOperationException("Antrenör, kendi randevusunu oluşturamaz.");
        }

        var service = await _serviceService.GetServiceByIdAsync(appointment.ServiceId);
        if (service == null)
        {
            return false;
        }

        var appointmentEndForCheck = appointment.AppointmentDate.TimeOfDay.Add(TimeSpan.FromMinutes(service.DurationInMinutes));

        var isAvailable = (await _availabilityRepository.GetAllAsync(
            a =>
                a.TrainerId == appointment.TrainerId &&
                a.Day == appointment.AppointmentDate.DayOfWeek &&
                a.StartTime <= appointment.AppointmentDate.TimeOfDay &&
                a.EndTime >= appointmentEndForCheck
            )).Any();

        if(!isAvailable)
        {
            return false;
        }

        var appointmentEnd = appointment.AppointmentDate.AddMinutes(service.DurationInMinutes);

        var conflictExists = (await _appointmentRepository.GetAllAsync(
            a =>
                a.TrainerId == appointment.TrainerId &&
                a.AppointmentDate == appointment.AppointmentDate.Date &&
                a.IsCancelled == false &&
                (
                    appointment.AppointmentDate < a.AppointmentDate.AddMinutes(a.Service.DurationInMinutes) &&
                    appointmentEnd > a.AppointmentDate
                )
            )).Any();

        if (conflictExists)
        {
            return false;
        }

        await _appointmentRepository.AddAsync(appointment);
        await _appointmentRepository.SaveAsync();
        return true;
    }

    public async Task<bool> ApproveAppointment(int appointmentId, string adminNotes)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
        if(appointment == null)
        {
            return false;
        }

        if(appointment.IsConfirmed || appointment.IsCancelled)
        {
            return false;
        }

        appointment.IsConfirmed = true;
        appointment.AdminNotes = adminNotes;
        await _appointmentRepository.UpdateAsync(appointment);
        await _appointmentRepository.SaveAsync();
        return true;
    }

    public async Task<bool> CancelAppointment(int appointmentId)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
        if(appointment == null)
        {
            return false;
        }

        appointment.IsCancelled = true;
        await _appointmentRepository.UpdateAsync(appointment);
        await _appointmentRepository.SaveAsync();
        return true;
    }



    public async Task<List<Appointment>> GetPendingAppointmentsAsync()
    {
        return await _appointmentRepository.GetAllAsync(a => !a.IsConfirmed && !a.IsCancelled);
    }




}
