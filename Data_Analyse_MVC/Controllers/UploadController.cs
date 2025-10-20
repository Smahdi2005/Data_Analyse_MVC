using Data_Analyse_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Data_Analyse_MVC.Controllers
{
    public class UploadController : Controller
    {
        private readonly AppDbContext _db;

        public UploadController(AppDbContext db)
        {
            _db = db;
        }

        // صفحه آپلود فایل
        [HttpGet]
        public IActionResult Upload()
        {
            return View(); // Views/Upload/Index.cshtml
        }

        // آپلود فایل با AJAX
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UploadFileAjax(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "فایلی انتخاب نشده است" });

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var uploadFile = new UploadFile
            {
                OriginalFileName = file.FileName,
                ContentType = file.ContentType,
                Length = file.Length,
                Data = ms.ToArray(),
                CreatedAtUtc = DateTime.UtcNow
            };

            _db.UploadFiles.Add(uploadFile);
            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "فایل با موفقیت آپلود شد" });
        }

        // بارگذاری لیست فایل‌ها برای PartialView
        [HttpGet]
        public IActionResult GetUserFilesPartial()
        {
            var files = _db.UploadFiles
                           .Include(f => f.AnalyseResults)
                           .OrderByDescending(f => f.CreatedAtUtc)
                           .ToList();

            return PartialView("_UserFileListPartial", files);
        }
    }
}
