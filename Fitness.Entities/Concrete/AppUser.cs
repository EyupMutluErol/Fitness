using Fitness.Entities.Abstract;
using Microsoft.AspNetCore.Identity;

namespace Fitness.Entities.Concrete;

public class AppUser:IdentityUser,IEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public double? Height { get; set; }
    public double? Weight { get; set; }
    public string? BodyType { get; set; }
    public string? ProfilePhotoUrl { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; }
}
