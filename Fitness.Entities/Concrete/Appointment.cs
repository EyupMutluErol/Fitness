using Fitness.Entities.Abstract;

namespace Fitness.Entities.Concrete;

public class Appointment:IEntity
{
    public int Id { get; set; }
    public string AppUserId { get; set; }
    public int TrainerId { get; set; }
    public int ServiceId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public bool IsConfirmed { get; set; }
    public bool IsCancelled { get; set; }
    public string AdminNotes { get; set; }


    public virtual AppUser AppUser { get; set; }
    public virtual Trainer Trainer { get; set; }
    public virtual Service Service { get; set; }
}
