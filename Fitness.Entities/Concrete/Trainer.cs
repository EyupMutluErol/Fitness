using Fitness.Entities.Abstract;

namespace Fitness.Entities.Concrete;

public class Trainer:IEntity
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Specialization { get; set; }
    public string Bio { get; set; }
    public string ProfileImageUrl { get; set; }

    
    public virtual ICollection<Availability> Availabilities { get; set; }
    public virtual ICollection<ServiceTrainer> ServiceTrainers { get; set; }
    public virtual ICollection<Appointment> Appointments { get; set; } 
    
}
