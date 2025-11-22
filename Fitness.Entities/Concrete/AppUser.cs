using Fitness.Entities.Abstract;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Fitness.Entities.Concrete;

public class AppUser:IdentityUser,IEntity
{
    [Required(ErrorMessage = "Ad zorunludur.")]
    [MaxLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "Soyad zorunludur.")]
    [MaxLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
    public string LastName { get; set; }
    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-posta adresi girin.")]
    [MaxLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir.")]
    public override string? Email { get; set; }
    [Required(ErrorMessage = "Telefon numarası zorunludur.")]
    [RegularExpression(@"^5[0-9]{9}$", ErrorMessage = "Telefon numarası 5xx xxx xx xx formatında olmalıdır.")]
    [MaxLength(10, ErrorMessage = "Telefon numarası en fazla 10 hane olmalıdır.")]
    public override string? PhoneNumber { get; set; }
    [Range(100, 250, ErrorMessage = "Boy 100cm ile 250cm arasında olabilir.")]
    public double? Height { get; set; }
    [Range(30, 300, ErrorMessage = "Kilo 30kg ile 300kg arasında olabilir.")]
    public double? Weight { get; set; }
    [MaxLength(50, ErrorMessage = "Vücut tipi en fazla 50 karakter olabilir.")]
    public string? BodyType { get; set; }
    [Url(ErrorMessage = "Geçersiz profil fotoğrafı URL'si.")]
    [MaxLength(255, ErrorMessage = "Profil fotoğrafı URL'si en fazla 255 karakter olabilir.")]
    public string? ProfilePhotoUrl { get; set; }

    public virtual Trainer? TrainerProfile { get; set; }
    public virtual ICollection<Appointment> Appointments { get; set; }
}
