using Data_Analyse_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Data_Analyse_MVC.Controllers
{
    public class AnalyseResultController : Controller
    {
        private readonly AppDbContext _db;

        public AnalyseResultController(AppDbContext db)
        {
            _db = db;
        }

        // PartialView تحلیل فایل برای AJAX
        [HttpGet]
        public IActionResult LoadAnalyseResultPartial(Guid analyseResultId)
        {
            var result = _db.AnalyseResults
                 .Include(r => r.UploadFile) // اضافه کردن Include
                 .FirstOrDefault(r => r.Id == analyseResultId);
            if (result == null)
                return Content("نتیجه تحلیل پیدا نشد");

            return PartialView("_AnalyseResultPartial", result);
        }

        // View کامل تحلیل
        [HttpGet]
        public IActionResult AnalyseResult(Guid analyseResultId)
        {
            var result = _db.AnalyseResults.FirstOrDefault(r => r.Id == analyseResultId);
            if (result == null)
            {
                TempData["Error"] = "نتیجه تحلیل پیدا نشد";
                return RedirectToAction();
            }

            return View(result);
        }
    }
}
