using System.Diagnostics;
using Fitness.Entities.Concrete; // AppUser için gerekli
using Fitness.Models;
using Microsoft.AspNetCore.Identity; // UserManager ve Rol kontrolü için gerekli
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks; // Index metodundaki async için gerekli

namespace Fitness.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager; // Yönlendirme için gerekli

        // Constructor güncellendi: ILogger ve UserManager alacak.
        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        // Ana sayfaya gelen isteði karþýlama metodu.
        // Rol bazlý yönlendirme bu metodun içinde yapýlýr.
        public async Task<IActionResult> Index()
        {
            // Kontrol 1: Kullanýcý oturum açmýþ mý?
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);

                // Kontrol 2: Admin rolü kontrolü
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    // Admin ise Admin Controller'ýnýn Index sayfasýna yönlendir.
                    return RedirectToAction("Index", "Admin");
                }

                // Kontrol 3: Member veya Trainer rolü kontrolü
                else if (await _userManager.IsInRoleAsync(user, "Member") || await _userManager.IsInRoleAsync(user, "Trainer"))
                {
                    // Üye/Antrenör ise Randevu Controller'ýnýn Index sayfasýna yönlendir.
                    return RedirectToAction("Index", "Appointment");
                }
            }

            // Kontrol 4: Kullanýcý oturum açmamýþsa (Anonymous), Login sayfasýna yönlendir.
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}