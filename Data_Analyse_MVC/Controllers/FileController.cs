using Data_Analyse_MVC.Models;
using Data_Analyse_MVC.Services;
using Data_Analyse_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Data_Analyse_MVC.Controllers
{
    public class FileController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IFileAnalyseService _fileAnalysService;

        public FileController(AppDbContext db, IFileAnalyseService fileAnalysService)
        {
            _db = db;
            _fileAnalysService = fileAnalysService;
        }
        public IActionResult ManageFiles()
        {
            var files = _db.UploadFiles.ToList();
            return View(files);



        }
        [HttpPost]
        public async Task<IActionResult> StartAnalys(Guid fileId)
        {
            var file = await _db.UploadFiles.FindAsync(fileId);
            if (file == null)
            {
                TempData["Error"] = "فایل پیدا نشد";
                return RedirectToAction(nameof(ManageFiles));
            }
            if (file.OriginalFileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                await _fileAnalysService.AnalyseTextFileAsync(file.Id, CancellationToken.None);
            }
            else if (file.OriginalFileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase) ||
                     file.OriginalFileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                await _fileAnalysService.AnalyseExelFileAsync(file.Id, CancellationToken.None);
            }
            else
            {
                TempData["Error"] = "فرمت فایل پشتیبانی نمی‌شود!";
                return RedirectToAction(nameof(ManageFiles));
            }

            TempData["Message"] = "تحلیل فایل شروع شد!";
            return RedirectToAction(nameof(ManageFiles));
        }



    }

}
