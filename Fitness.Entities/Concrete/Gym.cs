using Fitness.Entities.Abstract;

namespace Fitness.Entities.Concrete;

public class Gym:IEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string WorkingHours { get; set; }
}
