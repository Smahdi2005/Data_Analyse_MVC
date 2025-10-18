using ClosedXML.Excel;
using Data_Analyse_MVC.Models;
using Data_Analyse_MVC.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;



namespace Data_Analyse_MVC.Services
{
    public class FileAnalyseService : IFileAnalyseService
    {
        private readonly AppDbContext _db;
        public FileAnalyseService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<AnalyseResult?> AnalyseTextFileAsync(Guid uploadFileId, CancellationToken cancellationToken)
        {


            var file = await _db.UploadFiles.FindAsync(new object[] { uploadFileId }, cancellationToken);
            if (file == null) throw new Exception("File not found");

            string text = System.Text.Encoding.UTF8.GetString(file.Data ?? Array.Empty<byte>());


            text = text.Trim();
            text = Regex.Replace(text, @"[@#.,;:!?…]", "");
            text = text.ToLower();
            var stopWords = new[] { "که", "یا", "و" };

            foreach (var stopwords in stopWords)
            {
                string pattern = @"\b" + Regex.Escape(stopwords) + @"\b";

                text = Regex.Replace(text, pattern, "");
            }

            var words = text.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var sentences = Regex.Split(text, @"[.!?]+");



            int totalWords = words.Length;
            int totalSentences = sentences.Length;
            var wordFrequency = words.GroupBy(w => w)
                         .ToDictionary(g => g.Key, g => g.Count());

            var topWords = wordFrequency.OrderByDescending(kv => kv.Value)
                                        .Take(10)
                                        .ToList();
            var wordDensity = wordFrequency.ToDictionary(kv => kv.Key, kv => ((double)kv.Value / totalWords) * 100);

            foreach (var kv in wordFrequency)
            {
                string word = kv.Key;
                int count = kv.Value;

                double density = ((double)count / totalWords) * 100;
                wordDensity[word] = density;
            }
            var numericValues = words.Select(w => double.TryParse(w, out double n) ? (double?)n : null)
                                     .Where(n => n.HasValue)
                                     .Select(n => n.Value)
                                     .ToList();

            var numericAnalyse = numericValues.Count > 0 ? new
            {
                TotalNumbers = numericValues.Count,
                Sum = numericValues.Sum(),
                Average = numericValues.Average(),
                Min = numericValues.Min(),
                Max = numericValues.Max(),
                Values = numericValues
            } : null;


            var result = new AnalyseResult
            {
                Id = Guid.NewGuid(),
                UploadFileId = file.Id,
                ResultJson = System.Text.Json.JsonSerializer.Serialize(new
                {
                    TextAnalysis = new
                    {
                        TotalWords = totalWords,
                        TotalSentences = totalSentences,
                        TopWords = topWords,
                        WordDensity = wordDensity
                    },
                    NumericAnalysis = numericAnalyse
                }),
                CreatedAtUtc = DateTime.UtcNow
            };

            _db.AnalyseResults.Add(result);
            await _db.SaveChangesAsync(cancellationToken);

            return result;
        }

        public async Task<AnalyseResult> AnalyseExelFileAsync(Guid uploadFileId, CancellationToken cancellationToken)
        {
            var file = await _db.UploadFiles.FindAsync(new object[] { uploadFileId }, cancellationToken);
            if (file == null) throw new Exception("File not found");

            using (var stream = new MemoryStream(file.Data))
            using (var workbook = new XLWorkbook(stream))
            {
                var worksheet = workbook.Worksheets.First();
                var usedRange = worksheet.RangeUsed();

                var numericValues = new List<double>();
                var textValues = new List<string>();

                foreach (var row in usedRange.Rows())
                {
                    foreach (var cell in row.Cells())
                    {
                        var cellValue = cell.Value.ToString();
                        if (double.TryParse(cellValue, out double val))
                        {
                            numericValues.Add(val);
                        }
                        else
                        {
                            textValues.Add(cellValue);
                        }
                    }
                }

                // تحلیل عددی
                var numericAnalyse = numericValues.Count > 0 ? new
                {
                    TotalNumbers = numericValues.Count,
                    Sum = numericValues.Any() ? numericValues.Sum() : (double?)null,
                    Average = numericValues.Any() ? numericValues.Average() : (double?)null,
                    Min = numericValues.Any() ? numericValues.Min() : (double?)null,
                    Max = numericValues.Any() ? numericValues.Max() : (double?)null,
                    NumericValues = numericValues,
                    TotalText = textValues.Count,
                    TextValues = textValues
                } : null;

                // ذخیره در دیتابیس
                var result = new AnalyseResult
                {
                    Id = Guid.NewGuid(),
                    UploadFileId = file.Id,
                    ResultJson = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        TextAnalysis = new
                        {
                            TotalText = textValues.Count,
                            TextValues = textValues
                        },
                        NumericAnalysis = numericAnalyse
                    }),
                    CreatedAtUtc = DateTime.UtcNow
                };

                _db.AnalyseResults.Add(result);
                await _db.SaveChangesAsync(cancellationToken);

                return result;
            }
        }


    }
}
