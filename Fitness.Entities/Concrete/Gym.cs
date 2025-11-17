using Fitness.Entities.Abstract;
using System.ComponentModel.DataAnnotations;

namespace Fitness.Entities.Concrete;

public class Gym:IEntity
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Salon adı zorunludur.")]
    [MaxLength(100, ErrorMessage = "Salon adı en fazla 100 karakter olabilir.")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Adres zorunludur.")]
    [MaxLength(250, ErrorMessage = "Adres en fazla 250 karakter olabilir.")]
    public string Address { get; set; }
    [Required(ErrorMessage = "Telefon numarası zorunludur.")]
    [RegularExpression(@"^5[0-9]{9}$", ErrorMessage = "Telefon numarası 5xx xxx xx xx formatında, 10 haneli ve bitişik olmalıdır.")]
    public string PhoneNumber { get; set; }
    [Required(ErrorMessage = "Çalışma saatleri zorunludur.")]
    [MaxLength(100,ErrorMessage = "Çalışma saatleri en fazla 100 karakter olabilir.")]
    public string WorkingHours { get; set; }
}
