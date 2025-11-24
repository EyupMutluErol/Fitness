using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Fitness.Models;


public class UserDto
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public int TrainerProfileId { get; set; } 
}
public class UserListViewModel
{
    public List<UserDto> Users { get; set; }
}

public class UserEditViewModel
{
    public string Id { get; set; } // Hangi kullanıcıyı düzenlediğimizi gösteren ID

    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
    public string LastName { get; set; }

    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    // Rol Yönetimi Alanları
    [ValidateNever]
    public string CurrentRole { get; set; } // Kullanıcının şu anki rolü (display)

    // Admin'in seçebileceği tüm roller
    [ValidateNever]
    public List<IdentityRole> AvailableRoles { get; set; }

    // Adminin atamak istediği yeni rol (Formdan POST ile gelir)
    [Required(ErrorMessage = "Lütfen yeni bir rol seçiniz.")]
    public string SelectedRole { get; set; }
}