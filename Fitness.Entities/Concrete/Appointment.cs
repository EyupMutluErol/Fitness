using Fitness.Entities.Abstract;
using System.ComponentModel.DataAnnotations;

namespace Fitness.Entities.Concrete;

public class Appointment: IEntityWithId
{
    public int Id { get; set; }
    [Required]
    public string AppUserId { get; set; }
    [Required(ErrorMessage = "Antrenör seçimi zorunludur.")]
    public int TrainerId { get; set; }
    [Required(ErrorMessage = "Hizmet seçimi zorunludur.")]
    public int ServiceId { get; set; }
    [Required(ErrorMessage = "Randevu tarihi seçimi zorunludur.")]
    public DateTime AppointmentDate { get; set; }
    public bool IsConfirmed { get; set; }
    public bool IsCancelled { get; set; }
    [MaxLength(500, ErrorMessage = "Kullanıcı notları en fazla 500 karakter olabilir.")]
    public string? AdminNotes { get; set; }


    public virtual AppUser AppUser { get; set; }
    public virtual Trainer Trainer { get; set; }
    public virtual Service Service { get; set; }
}
