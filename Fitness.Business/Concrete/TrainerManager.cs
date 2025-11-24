using Fitness.Business.Abstract;
using Fitness.Data.Abstract;
using Fitness.Entities.Concrete;


namespace Fitness.Business.Concrete;

public class TrainerManager : ITrainerService
{
    private readonly ITrainerRepository _trainerRepository;
    private readonly IAppointmentRepository _appointmentRepository;

    public TrainerManager(ITrainerRepository trainerRepository, IAppointmentRepository appointmentRepository)
    {
        _trainerRepository = trainerRepository;
        _appointmentRepository = appointmentRepository;
    }
    public async Task AddTrainer(Trainer trainer)
    {
        var existingProfileByAppUser = await _trainerRepository.GetAsync(
        t => t.AppUserId == trainer.AppUserId
    );

        if (existingProfileByAppUser != null)
        {
            throw new InvalidOperationException("Bu kullanıcı zaten bir antrenör profiline sahiptir.");
        }

        // 2. Kaydetme
        await _trainerRepository.AddAsync(trainer);
        await _trainerRepository.SaveAsync();
    }
    public async Task UpdateTrainer(Trainer trainer)
    {
        if(trainer.Id <= 0)
        {
            throw new ArgumentException("Geçersiz antrenör ID'si.");
        }
        await _trainerRepository.UpdateAsync(trainer);
        await _trainerRepository.SaveAsync();
    }

    public async Task DeleteTrainer(int id)
    {
        var activeAppointments = await _appointmentRepository.GetAllAsync(a=>a.TrainerId == id && !a.IsCancelled);
        if(activeAppointments.Any())
        {
            throw new InvalidOperationException("Bu antrenörün aktif randevuları olduğu için silinemez.");
        }
        var trainerToDelete = await _trainerRepository.GetByIdAsync(id);
        if(trainerToDelete != null)
        {
            await _trainerRepository.DeleteAsync(trainerToDelete);
            await _trainerRepository.SaveAsync();
        }
    }
    public async Task<List<Trainer>> GetTrainersWithServices()
    {
        var allTrainers = await _trainerRepository.GetTrainersWithServiceAsync();
        return allTrainers;
    }
    public async Task<Trainer> GetTrainerDetails(int id)
    {
        var trainer = await _trainerRepository.GetTrainerWithDetailsAsync(id);
        if(trainer == null)
        {
            throw new KeyNotFoundException("Antrenör bulunamadı.");
        }
        return trainer;
    }

    public async Task<List<Trainer>> GetAvailableTrainers(DateTime date, int serviceId)
    {
        var trainers = await _trainerRepository.GetTrainersWithServiceAsync();
        var suitableTrainers = trainers.Where(t=>t.ServiceTrainers.Any(st=>st.ServiceId == serviceId) && t.Availabilities.Any(av => av.Day == date.DayOfWeek)).ToList();
        var existingAppointments = await _appointmentRepository.GetAllAsync(a=>a.AppointmentDate.Date == date.Date && !a.IsCancelled);
        var trainersWithConflicts = existingAppointments
                .Select(a => a.TrainerId) 
                .Distinct() 
                .ToList();
        var finalAvailableTrainers = suitableTrainers
                .Where(t => !trainersWithConflicts.Contains(t.Id))
                .ToList();

        return finalAvailableTrainers;
    }

    public async Task<Trainer> GetAsync(System.Linq.Expressions.Expression<Func<Trainer, bool>> filter)
    {
        return await _trainerRepository.GetAsync(filter);
    }




}
