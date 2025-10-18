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

        [HttpGet]
        public IActionResult Uploads()
        {
            return View("~/Views/uploads/uploads.cshtml");
        }

        [HttpPost]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> uploads(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "فایلی انتخاب نشده است.";
                return RedirectToAction("uploads");
            }

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var uploadFile = new UploadFile
            {
                OriginalFileName = file.FileName,
                ContentType = file.ContentType,
                Length = file.Length,
                Data = ms.ToArray()
            };

            _db.UploadFiles.Add(uploadFile);
            await _db.SaveChangesAsync();

            TempData["Message"] = "فایل با موفقیت آپلود شد!";
            return RedirectToAction("ManageFiles");
        }

        public IActionResult ManageFiles()
        {
            var files = _db.UploadFiles.ToList();
            return View("~/Views/ManageFiles/ManageFiles.cshtml", files);



        }
        [HttpPost]
        public async Task<AnalyseResult> StartAnalyse(Guid fileId)
        {
            var file = await _db.UploadFiles.FindAsync(fileId);
            AnalyseResult? result = null;
            if (file == null)
            {
                TempData["Error"] = "فایل پیدا نشد";
                
            }
            if (file.OriginalFileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                result = await _fileAnalysService.AnalyseTextFileAsync(file.Id, CancellationToken.None);

                TempData["Message"] = "تحلیل فایل انجام شد!";
               
            }
            else if (file.OriginalFileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase) ||
                     file.OriginalFileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                result = await _fileAnalysService.AnalyseExelFileAsync(file.Id, CancellationToken.None);
            }
            else
            {
                TempData["Error"] = "فرمت فایل پشتیبانی نمی‌شود!";
                
            }
            if (result == null)
            {
                TempData["Error"] = "تحلیل فایل انجام نشد!";
                
            }
            return result;

            // هدایت به ویوی AnalyseResult و پاس دادن نتیجه

        }

        public IActionResult AnalyseResult(Guid analyseResultId)
        {
            var result = _db.AnalyseResults.FirstOrDefault(r => r.Id == analyseResultId);
            if (result == null)
            {
                TempData["Error"] = "نتیجه تحلیل پیدا نشد!";
                return RedirectToAction(nameof(ManageFiles));
            }

            return View("~/Views/AnalyseResult/AnalyseResult.cshtml", result); // نمایش ویو AnalyseResult
        }
    }

}

