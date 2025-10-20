using Data_Analyse_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Data_Analyse_MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        // صفحه مدیریت فایل‌ها
        [HttpGet]
        public IActionResult ManageFiles()
        {
            var files = _db.UploadFiles.Include(f => f.AnalyseResults)
                                       .OrderByDescending(f => f.CreatedAtUtc)
                                       .ToList();
            return View("Admin",files);
        }

    }
}
