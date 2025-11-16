using Fitness.Entities.Abstract;

namespace Fitness.Entities.Concrete;

public class Service:IEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int DurationInMinutes { get; set; }
    public decimal Price { get; set; }


    public virtual ICollection<ServiceTrainer> ServiceTrainers { get; set; }
    public virtual ICollection<Appointment> Appointments { get; set; }
}
