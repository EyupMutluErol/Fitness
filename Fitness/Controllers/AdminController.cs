using Fitness.Business.Abstract;
using Fitness.Entities.Concrete;
using Fitness.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fitness.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ITrainerService _trainerService;
    private readonly IServiceService _serviceService;
    private readonly IGymService _gymService;
    private readonly IAppointmentService _appointmentService;
    private readonly UserManager<AppUser> _userManager; 
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(ITrainerService trainerService, IServiceService serviceService, IGymService gymService, IAppointmentService appointmentService,UserManager<AppUser> userManager, IWebHostEnvironment webHostEnvironment, RoleManager<IdentityRole> roleManager)
    {
        _trainerService = trainerService;
        _serviceService = serviceService;
        _gymService = gymService;
        _appointmentService = appointmentService;
        _userManager = userManager;
        _webHostEnvironment = webHostEnvironment;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        // ViewModel'i try bloğunun dışında hemen initialize (ön tanımlama) ediyoruz.
        var viewModel = new AdminDashboardViewModel
        {
            TotalTrainers = 0,
            TotalServices = 0,
            PendingAppointments = 0,
            TotalMembers = 0,
            TotalGyms = 0
        };

        try
        {
            // 1. Verileri Çekme ve Sayma
            var totalTrainers = (await _trainerService.GetTrainersWithServices()).Count;
            var totalServices = (await _serviceService.GetAllServicesAsync()).Count;

            // AppointmentService'e eklediğimiz özel metodu çağırıyoruz.
            var pendingAppointments = (await _appointmentService.GetPendingAppointmentsAsync()).Count();

            // Toplam Üye Sayısını Alma (UserManager'dan)
            var totalMembers = await _userManager.Users.CountAsync();

            // Toplam Salon Sayısını Alma
            var totalGyms = (await _gymService.GetAllGymsAsync()).Count;

            // 2. ViewModel'i Doldurma
            viewModel.TotalTrainers = totalTrainers;
            viewModel.TotalServices = totalServices;
            viewModel.PendingAppointments = pendingAppointments;
            viewModel.TotalMembers = totalMembers;
            viewModel.TotalGyms = totalGyms;

            // 3. Başarılı View Dönüşü
            return View(viewModel);
        }
        catch (Exception ex)
        {
            // Hatanın tam olarak nerede oluştuğunu görebilmek için hata mesajını View'a taşıyalım.
            ViewBag.ErrorMessage = "Dashboard verileri yüklenirken KRİTİK HATA oluştu: " + ex.Message;

            // Hata durumunda, NullReference hatasını önlemek için sıfırlanmış ViewModel'i döndür.
            return View(viewModel);
        }
    }


    // Trainer Management
    [HttpGet]
    public async Task<IActionResult> TrainerList()
    {
        var trainers = await _trainerService.GetTrainersWithServices();
        if (TempData["ErrorMessage"] != null)
        {
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
        }
        if (TempData["SuccessMessage"] != null)
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
        }

        return View("Trainer/List", trainers);
    }

    // GET: /Admin/UpgradeList (Antrenör Adaylarını Listeleme)
    [HttpGet]
    public async Task<IActionResult> UpgradeList()
    {
        var users = await _userManager.Users.ToListAsync();
        var viewModelList = new List<UserToTrainerView>();
        var adminRoleName = "Admin";
        var trainerRoleName = "Trainer";

        // Sadece Member rolünde olan (veya rolsüz olan) kullanıcıları filtrele
        foreach (var user in users)
        {
            if (!await _userManager.IsInRoleAsync(user, adminRoleName) &&
                !await _userManager.IsInRoleAsync(user, trainerRoleName))
            {
                // Ayrıca, bu AppUser'ın zaten bir Trainer profili olup olmadığını da kontrol etmeliyiz.
                var existingProfile = await _trainerService.GetAsync(t => t.AppUserId == user.Id);

                if (existingProfile == null)
                {
                    viewModelList.Add(new UserToTrainerView
                    {
                        Id = user.Id,
                        FullName = $"{user.FirstName} {user.LastName}",
                        Email = user.Email
                    });
                }
            }
        }

        return View("Trainer/UpgradeList", new UserSelectionViewModel { Users = viewModelList });
    }


    // GET: /Admin/AssignTrainerProfile?userId=... (Profil Oluşturma Formu - Yükseltme)
    [HttpGet]
    public async Task<IActionResult> AssignTrainerProfile(string userId)
    {
        if (string.IsNullOrEmpty(userId)) return NotFound();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        // ViewModel'i oluştururken kullanıcının ad/soyadını alıyoruz.
        var model = new Trainer
        {
            AppUserId = user.Id, // YENİ FK SET EDİLDİ
            FirstName = user.FirstName,
            LastName = user.LastName
        };

        return View("Trainer/AssignProfile", model);
    }

    // POST: /Admin/AssignTrainerProfile (Rol Atama ve Profili Kaydetme)
    [HttpPost]
    public async Task<IActionResult> AssignTrainerProfile(Trainer model, IFormFile? file)
    {
        // Dosya URL'sini ve AppUserId'yi ModelState'ten kaldırıyoruz çünkü formdan gelmiyor.
        ModelState.Remove(nameof(Trainer.ProfileImageUrl));
        ModelState.Remove(nameof(Trainer.AppUserId));

        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.AppUserId);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Hedef kullanıcı bulunamadı.");
                return View("Trainer/AssignProfile", model);
            }

            // 1. ROL ATAMA VE LİNKLEME
            await _userManager.AddToRoleAsync(user, "Trainer"); // Rolü yükselt.

            // 2. Dosya Kayıt Mantığı
            if (file != null && file.Length > 0)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "img/trainers");
                var filePath = Path.Combine(folderPath, uniqueFileName);

                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                model.ProfileImageUrl = "/img/trainers/" + uniqueFileName;
            }

            // 3. Trainer Profilini Kaydetme
            await _trainerService.AddTrainer(model);

            TempData["SuccessMessage"] = $"'{model.FirstName} {model.LastName}' kullanıcısı başarıyla antrenör rolüne yükseltildi.";
            return RedirectToAction("TrainerList");
        }

        return View("Trainer/AssignProfile", model);
    }

    // Trainer Edit (Mevcut profil bilgilerini düzenleme)
    [HttpGet]
    public async Task<IActionResult> TrainerEdit(int? id)
    {
        if (id == null) return NotFound();
        var trainer = await _trainerService.GetTrainerDetails(id.Value);
        if (trainer == null) return NotFound();

        return View("Trainer/Edit", trainer);
    }

    [HttpPost]
    public async Task<IActionResult> TrainerEdit(Trainer model)
    {
        ModelState.Remove(nameof(Trainer.ProfileImageUrl)); // File input boş gelebilir
        ModelState.Remove(nameof(Trainer.AppUserId)); // Sistem tarafından yönetiliyor

        if (ModelState.IsValid)
        {
            try
            {
                // Dosya yükleme logic'i buraya tekrar gelmelidir (Eğer dosya yüklenmişse).
                // Basitlik için şimdilik sadece mevcut kaydı güncelliyoruz.
                await _trainerService.UpdateTrainer(model);
                TempData["SuccessMessage"] = $"'{model.FirstName} {model.LastName}' antrenörü başarıyla güncellendi.";
                return RedirectToAction("TrainerList");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Antrenör güncellenirken bir hata oluştu: " + ex.Message);
            }
        }
        return View("Trainer/Edit", model);
    }

    [HttpPost]
    public async Task<IActionResult> TrainerDelete(int id)
    {
        // 1. Trainer profilini bulma (Mesaj ve varlık kontrolü için)
        var trainer = await _trainerService.GetTrainerDetails(id);
        if (trainer == null)
        {
            TempData["ErrorMessage"] = "Antrenör bulunamadı.";
            return RedirectToAction("TrainerList");
        }

        // 2. Bağlı olduğu AppUser hesabını bulma (Silmek için zorunludur)
        var appUser = await _userManager.FindByIdAsync(trainer.AppUserId);

        try
        {
            // 3. Trainer Profilini Sil (Business Layer aktif randevuları burada kontrol eder.)
            await _trainerService.DeleteTrainer(id);

            // 4. KULLANICI HESABINI TAMAMEN SİLME (IDENTITY PURGE)
            // Bu komut, kullanıcının rollerini, login bilgilerini ve AppUser kaydını kökten siler.
            if (appUser != null)
            {
                await _userManager.DeleteAsync(appUser);
            }

            // 5. BAŞARI: Mesajı güncelledik.
            TempData["SuccessMessage"] = $"Antrenör '{trainer.FirstName} {trainer.LastName}' başarıyla sistemden TAMAMEN silindi.";
            return RedirectToAction("TrainerList");
        }
        catch (InvalidOperationException ex)
        {
            // İş Kuralı Hatası (Aktif randevu varsa silinemez).
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            // Genel hata yakalama
            TempData["ErrorMessage"] = $"Antrenör silinirken bir hata oluştu: {ex.Message}";
        }
        return RedirectToAction("TrainerList");
    }



    // Service Management
    [HttpGet]
    public async Task<IActionResult> ServiceList()
    {
        var services = await _serviceService.GetServicesWithTrainersAsync();

        if (TempData["ErrorMessage"] != null)
        {
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
        }
        if (TempData["SuccessMessage"] != null)
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
        }

        return View("Service/List",services);
    }

    [HttpGet]
    public IActionResult ServiceCreate()
    {
        return View("Service/Create",new Service());
    }

    [HttpPost]
    public async Task<IActionResult> ServiceCreate(Service model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _serviceService.AddServiceAsync(model);
                TempData["SuccessMessage"] = $"'{model.Name}' hizmeti başarıyla eklendi.";
                return RedirectToAction("ServiceList");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Hizmet eklenirken bir hata oluştu.");
            }
        }
        return View("Service/Create",model);
    }

    [HttpGet]
    public async Task<IActionResult> ServiceEdit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var service = await _serviceService.GetServiceByIdAsync(id.Value);
        if (service == null)
        {
            return NotFound();
        }
        return View("Service/Edit",service);
    }

    [HttpPost]
    public async Task<IActionResult> ServiceEdit(Service model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _serviceService.UpdateServiceAsync(model);
                TempData["SuccessMessage"] = $"'{model.Name}' hizmeti başarıyla güncellendi.";
                return RedirectToAction("ServiceList");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Hizmet güncellenirken bir hata oluştu.");
            }
        }
        return View("Service/Edit", model);
    }

    [HttpPost]
    public async Task<IActionResult> ServiceDelete(int id)
    {
        var service = await _serviceService.GetServiceByIdAsync(id);
        if (service == null)
        {
            TempData["ErrorMessage"] = "Hizmet bulunamadı.";
            return RedirectToAction("ServiceList");
        }
        try
        {
            await _serviceService.DeleteServiceAsync(id);
            TempData["SuccessMessage"] = $"'{service.Name}' hizmeti başarıyla silindi.";
            return RedirectToAction("ServiceList");
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Hizmet silinirken bir hata oluştu.";
        }
        return RedirectToAction("ServiceList");
    }

    // Gym Management
    [HttpGet]
    public async Task<IActionResult> GymList()
    {
        var gyms = await _gymService.GetAllGymsAsync();
        if (TempData["ErrorMessage"] != null)
        {
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
        }
        if (TempData["SuccessMessage"] != null)
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
        }
        return View("Gym/List",gyms);
    }

    [HttpGet]
    public IActionResult GymCreate()
    {
        return View("Gym/Create",new Gym());
    }

    [HttpPost]
    public async Task<IActionResult> GymCreate(Gym model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _gymService.AddGymAsync(model);
                TempData["SuccessMessage"] = $"'{model.Name}' salonu başarıyla tanımlandı."; 
                return RedirectToAction("GymList");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Spor salonu eklenirken bir hata oluştu.");
            }
        }
        return View("Gym/Create",model);
    }

    [HttpGet] 
    public async Task<IActionResult> GymEdit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var gym = await _gymService.GetGymDetailsAsync(id.Value);
        if (gym == null)
        {
            return NotFound();
        }
        return View("Gym/Edit",gym);
    }

    [HttpPost]
    public async Task<IActionResult> GymEdit(Gym model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _gymService.UpdateGymAsync(model);
                TempData["SuccessMessage"] = $"'{model.Name}' salonu başarıyla güncellendi."; 
                return RedirectToAction("GymList");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Spor salonu güncellenirken bir hata oluştu.");
            }
        }
        return View("Gym/Edit",model);
    }

    [HttpPost]
    public async Task<IActionResult> GymDelete(int id)
    {
        var gym = await _gymService.GetGymDetailsAsync(id);
        if (gym == null)
        {
            TempData["ErrorMessage"] = "Spor salonu bulunamadı.";
            return RedirectToAction("GymList");
        }
        try
        {
            await _gymService.DeleteGymAsync(id);
            TempData["SuccessMessage"] = $"'{gym.Name}' salonu başarıyla silindi."; 
            return RedirectToAction("GymList");
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Spor salonu silinirken bir hata oluştu.";
        }
        return RedirectToAction("GymList");
    }


    // Member Management
    [HttpGet]
    public async Task<IActionResult> MemberList()
    {
        var users = await _userManager.Users.ToListAsync();
        var viewModelList = new List<UserDto>(); 

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            viewModelList.Add(new UserDto
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Role = string.Join(", ", roles),
                TrainerProfileId = user.TrainerProfile?.Id ?? 0 
            });
        }

        return View("Member/List", new UserListViewModel { Users = viewModelList });
    }
    [HttpGet]
    public async Task<IActionResult> MemberEdit(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        // 1. Mevcut Rolü Bulma
        var userRoles = await _userManager.GetRolesAsync(user);

        // 2. Tüm Rolleri Çekme
        var allRoles = _roleManager.Roles.ToList();

        // 3. ViewModel'i doldurma
        var model = new UserEditViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            CurrentRole = userRoles.FirstOrDefault() ?? "Member", // İlk rolü göster
            AvailableRoles = allRoles
        };

        return View("Member/Edit", model);
    }

    // POST: /Admin/MemberEdit (Kullanıcı Rolünü ve Bilgilerini Güncelle)
    [HttpPost]
    public async Task<IActionResult> MemberEdit(UserEditViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            // 1. Kullanıcının Temel Bilgilerini Güncelleme (AppUser)
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;

            // 2. ROL GÜNCELLEME İŞLEMİ (Tek Rol Politikası)
            var currentRoles = await _userManager.GetRolesAsync(user);
            var resultRemove = await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!resultRemove.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Mevcut roller kaldırılamadı.");
                // View'a geri dönerken rol listesini tekrar çekiyoruz.
                model.AvailableRoles = _roleManager.Roles.ToList();
                return View("Member/Edit", model);
            }

            // Yeni Rolü Ekleme
            var resultAdd = await _userManager.AddToRoleAsync(user, model.SelectedRole);

            // Kullanıcı nesnesini kaydetme
            var resultUpdate = await _userManager.UpdateAsync(user);

            if (resultAdd.Succeeded && resultUpdate.Succeeded)
            {
                // -------------------------------------------------------------
                // KRİTİK FİX: TRAINER PROFİLİNİ OLUŞTURMA
                // -------------------------------------------------------------
                if (model.SelectedRole == "Trainer")
                {
                    // Profil zaten var mı kontrol et (Tekrarlanan kayıtları önlemek için)
                    var existingTrainerProfile = await _trainerService.GetAsync(t => t.AppUserId == user.Id);

                    if (existingTrainerProfile == null)
                    {
                        // Minimal bir Trainer Entity'si oluşturulur.
                        var newTrainer = new Trainer
                        {
                            AppUserId = user.Id,
                            FirstName = user.FirstName, // AppUser'dan kopyalanan ad
                            LastName = user.LastName, // AppUser'dan kopyalanan soyad
                            Specialization = "Yönetici Ataması" // Varsayılan değer
                        };
                        await _trainerService.AddTrainer(newTrainer); // Profil Business katmanına kaydedilir.
                    }
                }
                // -------------------------------------------------------------

                TempData["SuccessMessage"] = $"{model.FirstName} {model.LastName} kullanıcısının rolü başarıyla '{model.SelectedRole}' olarak güncellendi.";
                return RedirectToAction("MemberList");
            }

            ModelState.AddModelError(string.Empty, "Rol güncelleme sırasında hata oluştu.");
        }

        // Hata durumunda View'a tekrar yükleme
        model.AvailableRoles = _roleManager.Roles.ToList();
        return View("Member/Edit", model);
    }

    // AdminController.cs içinde

    // POST: /Admin/MemberDelete/{id} (Kullanıcıyı Tamamen Silme)
    [HttpPost]
    public async Task<IActionResult> MemberDelete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            TempData["ErrorMessage"] = "Silinecek kullanıcı bulunamadı.";
            return RedirectToAction("MemberList");
        }

        try
        {
            // UserManager'ı kullanarak AppUser kaydını (ve rollerini) siler.
            // Bu işlem, kullanıcıya ait tüm Identity verilerini temizler.
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"'{user.Email}' kullanıcısı sistemden başarıyla silindi.";
            }
            else
            {
                // Identity'den gelen bir hata olursa (örn: rol kaldırılamazsa)
                TempData["ErrorMessage"] = "Kullanıcı silinirken bir hata oluştu: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }
        }
        catch (Exception ex)
        {
            // Randevu kısıtlaması hatası (OnDelete Restrict) buradan yakalanır.
            TempData["ErrorMessage"] = "Kullanıcı silinirken bir hata oluştu: Bu kullanıcıya ait aktif randevular veya bağımlı kayıtlar mevcut olabilir. " + ex.Message;
        }

        return RedirectToAction("MemberList");
    }
}
