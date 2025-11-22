using Fitness.Business.Abstract;
using Fitness.Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fitness.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ITrainerService _trainerService;
    private readonly IServiceService _serviceService;
    private readonly IGymService _gymService;

    public AdminController(ITrainerService trainerService, IServiceService serviceService, IGymService gymService)
    {
        _trainerService = trainerService;
        _serviceService = serviceService;
        _gymService = gymService;
    }

    public IActionResult Index()
    {
        return View();
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

        return View("Trainer/List",trainers);
    }

    [HttpGet]
    public IActionResult TrainerCreate()
    {
        return View("Trainer/Create",new Trainer());
    }

    [HttpPost]
    public async Task<IActionResult> TrainerCreate(Trainer model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _trainerService.AddTrainer(model);
                TempData["SuccessMessage"] = $"'{model.FirstName} {model.LastName}' antrenörü başarıyla eklendi."; 
                return RedirectToAction("TrainerList");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Antrenör eklenirken beklenmeyen bir hata oluştu.");
            }
        }
        return View("Trainer/Create",model);
    }

    [HttpGet]
    public async Task<IActionResult> TrainerEdit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var trainer = await _trainerService.GetTrainerDetails(id.Value);

        if (trainer == null)
        {
            return NotFound();
        }
        return View("Trainer/Edit",trainer);
    }

    [HttpPost]
    public async Task<IActionResult> TrainerEdit(Trainer model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _trainerService.UpdateTrainer(model);
                TempData["SuccessMessage"] = $"'{model.FirstName} {model.LastName}' antrenörü başarıyla güncellendi.";
                return RedirectToAction("TrainerList");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Antrenör güncellenirken bir hata oluştu.");
            }
        }
        return View("Trainer/Edit",model);
    }

    [HttpPost]
    public async Task<IActionResult> TrainerDelete(int id)
    {
        var trainer = await _trainerService.GetTrainerDetails(id);
        if(trainer == null)
        {
            TempData["ErrorMessage"] = "Antrenör bulunamadı.";
            return RedirectToAction("TrainerList");
        }
        try
        {
            await _trainerService.DeleteTrainer(id);
            TempData["SuccessMessage"] = $"Antrenör '{trainer.FirstName} {trainer.LastName}' başarıyla silindi.";
            return RedirectToAction("TrainerList");
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Antrenör silinirken bir hata oluştu.";
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
}
