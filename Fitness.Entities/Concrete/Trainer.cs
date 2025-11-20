using Fitness.Entities.Abstract;
using System.ComponentModel.DataAnnotations;

namespace Fitness.Entities.Concrete;

public class Trainer: IEntityWithId
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Antrenör adı zorunludur.")]
    [MaxLength(50, ErrorMessage = "Antrenör adı en fazla 50 karakter olabilir.")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "Antrenör soyadı zorunludur.")]
    [MaxLength(50, ErrorMessage = "Antrenör soyadı en fazla 50 karakter olabilir.")]
    public string LastName { get; set; }
    [Required(ErrorMessage = "Uzmanlık alanı zorunludur.")]
    [MaxLength(100, ErrorMessage = "Uzmanlık alanı en fazla 100 karakter olabilir.")]
    public string Specialization { get; set; }
    [Required(ErrorMessage = "Kullanıcı ID'si zorunludur.")]
    public string AppUserId { get; set; }
    [MaxLength(1000, ErrorMessage = "Biyografi en fazla 1000 karakter olabilir.")]
    public string? Bio { get; set; }
    [Url(ErrorMessage = "Geçersiz profil resmi URL'si.")]
    [MaxLength(255, ErrorMessage = "Profil resmi URL'si en fazla 255 karakter olabilir.")]
    public string? ProfileImageUrl { get; set; }

    
    public virtual ICollection<Availability> Availabilities { get; set; }
    public virtual ICollection<ServiceTrainer> ServiceTrainers { get; set; }
    public virtual ICollection<Appointment> Appointments { get; set; } 
    public virtual AppUser AppUser { get; set; }

    }
