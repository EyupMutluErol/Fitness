using Fitness.Entities.Abstract;

namespace Fitness.Entities.Concrete;

public class ServiceTrainer:IEntity
{
    public int ServiceId { get; set; }
    public int TrainerId { get; set; }

    public virtual Service Service { get; set; }
    public virtual Trainer Trainer { get; set; }
}
