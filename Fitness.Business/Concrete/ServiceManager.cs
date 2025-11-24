using Fitness.Business.Abstract;
using Fitness.Data.Abstract;
using Fitness.Entities.Concrete;

namespace Fitness.Business.Concrete;

public class ServiceManager:IServiceService
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IAppointmentRepository _appointmentRepository;

    public ServiceManager(IServiceRepository serviceRepository, IAppointmentRepository appointmentRepository)
    {
        _serviceRepository = serviceRepository;
        _appointmentRepository = appointmentRepository;
    }

    public async Task AddServiceAsync(Service service)
    {
        var existingService = await _serviceRepository.GetAllAsync(s => s.Name == service.Name);
        if (existingService != null) 
        {
            throw new InvalidOperationException("Aynı isimde bir hizmet zaten mevcut.");
        }
        if (service.Price < 0)
        {
            throw new ArgumentException("Hizmet fiyatı negatif olamaz.");
        }
        await _serviceRepository.AddAsync(service);
        await _serviceRepository.SaveAsync();
    }
    public async Task UpdateServiceAsync(Service service)
    {
        await _serviceRepository.UpdateAsync(service);
        await _serviceRepository.SaveAsync();
    }

    public async Task DeleteServiceAsync(int id)
    {
        var activeAppointments = await _appointmentRepository.GetAllAsync(a => a.ServiceId == id && !a.IsCancelled && a.AppointmentDate >= DateTime.Now);

        if(activeAppointments.Any())
        {
            throw new InvalidOperationException("Bu hizmete ait aktif randevular bulunduğu için silinemez.");
        }

        var serviceToDelete = await _serviceRepository.GetByIdAsync(id);    
        if(serviceToDelete != null)
        {
            await _serviceRepository.DeleteAsync(serviceToDelete);
            await _serviceRepository.SaveAsync();
        }
    }
    public async Task<Service> GetServiceByIdAsync(int id)
    {
        return await _serviceRepository.GetByIdAsync(id);
    }

    public async Task<List<Service>> GetAllServicesAsync()
    {
        return await _serviceRepository.GetAllAsync();
    }

    public async Task<List<Service>> GetServicesWithTrainersAsync()
    {
        return await _serviceRepository.GetServicesWithTrainersAsync();
    }

    
}
