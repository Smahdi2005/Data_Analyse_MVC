using Data_Analyse_MVC.Models;
using Data_Analyse_MVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Data_Analyse_MVC.Controllers
{
    public class FileController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IFileAnalyseService _fileAnalyseService;

        public FileController(AppDbContext db, IFileAnalyseService fileAnalyseService)
        {
            _db = db;
            _fileAnalyseService = fileAnalyseService;
        }

        // لیست فایل‌ها و وضعیت آن‌ها (کاربر)
        [HttpGet]
        public IActionResult UserFiles()
        {
            var files = _db.UploadFiles.Include(f => f.AnalyseResults)
                                       .OrderByDescending(f => f.CreatedAtUtc)
                                       .ToList();
            return PartialView("_UserFileListPartial", files);
        }

        // شروع تحلیل فایل
        [HttpPost]
        public async Task<JsonResult> StartAnalyse(Guid fileId)
        {
            var file = await _db.UploadFiles.FindAsync(fileId);
            if (file == null)
                return Json(new { success = false, message = "فایل پیدا نشد" });

            AnalyseResult? result = null;
            if (file.OriginalFileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                result = await _fileAnalyseService.AnalyseTextFileAsync(file.Id, CancellationToken.None);
            else if (file.OriginalFileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase) ||
                     file.OriginalFileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                result = await _fileAnalyseService.AnalyseExelFileAsync(file.Id, CancellationToken.None);
            else
                return Json(new { success = false, message = "فرمت فایل پشتیبانی نمی‌شود" });

            return Json(new { success = true, id = result.Id });
        }
    }
}
