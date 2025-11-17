using Fitness.Entities.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitness.Entities.Concrete;

public class Service:IEntity
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Hizmet adı zorunludur.")]
    [MaxLength(100, ErrorMessage = "Hizmet adı en fazla 100 karakter olabilir.")]
    public string Name { get; set; }
    [MaxLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
    public string? Description { get; set; }
    [Required(ErrorMessage = "Süre zorunludur.")]
    [Range(15, 180, ErrorMessage = "Süre 15 ile 180 dakika arasında olmalıdır.")]
    public int DurationInMinutes { get; set; }
    [Required(ErrorMessage = "Ücret zorunludur.")]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 10000, ErrorMessage = "Ücret 0 ile 10,000 arasında olmalıdır.")]
    public decimal Price { get; set; }


    public virtual ICollection<ServiceTrainer> ServiceTrainers { get; set; }
    public virtual ICollection<Appointment> Appointments { get; set; }
}
