using Fitness.Entities.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitness.Entities.Concrete;


public class Availability: IEntityWithId
{
    public int Id { get; set; }
    public int TrainerId { get; set; }
    public DayOfWeek Day { get; set; }
    [Column(TypeName = "time")]
    public TimeSpan StartTime { get; set; }
    [Column(TypeName = "time")]
    public TimeSpan EndTime { get; set; }


    public virtual Trainer Trainer { get; set; }
}
