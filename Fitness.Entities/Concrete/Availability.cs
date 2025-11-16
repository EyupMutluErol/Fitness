using Fitness.Entities.Abstract;

namespace Fitness.Entities.Concrete;


public class Availability:IEntity 
{
    public int Id { get; set; }
    public int TrainerId { get; set; }
    public DayOfWeek Day { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }


    public virtual Trainer Trainer { get; set; }
}
