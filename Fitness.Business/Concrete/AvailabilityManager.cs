using Fitness.Business.Abstract;
using Fitness.Data.Abstract;
using Fitness.Entities.Concrete;

namespace Fitness.Business.Concrete;

public class AvailabilityManager : IAvailabilityService
{
    private readonly IAvailabilityRepository _availabilityRepository;   
    private readonly IAppointmentRepository _appointmentRepository;

    public AvailabilityManager(IAvailabilityRepository availabilityRepository, IAppointmentRepository appointmentRepository)
    {
        _availabilityRepository = availabilityRepository;
        _appointmentRepository = appointmentRepository;
    }
    public async Task<List<Availability>> GetTrainerAvailabilityAsync(int trainerId)
    {
        return await _availabilityRepository.GetAllAsync(a=>a.TrainerId == trainerId);
    }

    public async Task<bool> AddAvailabilityBlockAsync(Availability availability)
    {
        var appointmentsInBlock = await _appointmentRepository.GetAllAsync(
            a =>
                a.TrainerId == availability.TrainerId &&
                a.AppointmentDate.DayOfWeek == availability.Day &&
                a.AppointmentDate.TimeOfDay >= availability.StartTime &&
                a.AppointmentDate.TimeOfDay < availability.EndTime &&
                a.IsCancelled == false
            );

        if (appointmentsInBlock.Any())
        {
            throw new InvalidOperationException("Bu zaman aralığında mevcut randevular bulunduğu için uygunluk bloğu eklenemez.");
        }
        await _availabilityRepository.AddAsync(availability);
        await _availabilityRepository.SaveAsync();
        return true;
    }

    

    public async Task<bool> RemoveAvailabilityBlockAsync(int availabilityId)
    {
        var availability = await _availabilityRepository.GetByIdAsync(availabilityId);
        if(availability == null)
        {
            return false;
        }

        var appointmentnInFuture = await _appointmentRepository.GetAllAsync(
            a =>
                a.TrainerId == availability.TrainerId &&
                a.AppointmentDate.DayOfWeek == availability.Day &&
                a.AppointmentDate >= DateTime.Now
            );

        if(appointmentnInFuture.Any())
        {
            throw new InvalidOperationException("Bu uygunluk bloğuna bağlı gelecekteki randevular bulunduğu için silinemez.");
        }

        await _availabilityRepository.DeleteAsync(availability);
        await _availabilityRepository.SaveAsync();
        return true;
    }
}
