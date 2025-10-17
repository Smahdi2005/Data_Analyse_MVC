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

        public async Task AnalyseTextFileAsync(Guid uploadFileId, CancellationToken cancellationToken)
        {
            var file = await _db.UploadFiles.FindAsync(new object[] { uploadFileId }, cancellationToken);
            if (file == null) return;

            string text = System.Text.Encoding.UTF8.GetString(file.Data);


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



            int totalwords = words.Length;
            double avWordLength = words.Length > 0 ? words.Average(w => w.Length) : 0;
            int totalsentences = sentences.Length;
            var wordFrequency = new Dictionary<string, int>();

            foreach (var word in words)
            {
                if (wordFrequency.ContainsKey(word))
                {
                    wordFrequency[word]++;
                }
                else
                {
                    wordFrequency[word] = 1;
                }
            }
            var topWords = wordFrequency.ToList();
            topWords.Sort((a, b) => b.Value.CompareTo(a.Value));
            topWords = topWords.Take(10).ToList();


            var wordDensity = new Dictionary<string, double>();

            foreach (var kv in wordFrequency)
            {
                string word = kv.Key;
                int count = kv.Value;

                double density = ((double)count / totalwords) * 100;
                wordDensity[word] = density;
            }



            var numericValues = new List<double>();

            foreach (var word in words)
            {
                if (double.TryParse(word, out double num))
                {
                    numericValues.Add(num);
                }
            }

            if (numericValues.Count > 0)
            {
                double sum = numericValues.Sum();
                double average = numericValues.Average();
                double min = numericValues.Min();
                double max = numericValues.Max();
                int totalNumbers = numericValues.Count;

                var numericAnalyse = new
                {
                    TotalNumbers = totalNumbers,
                    sum = sum,
                    Average = average,
                    Min = min,
                    Max = max,
                    Values = numericValues
                };

                var result = new AnalyseResult
                {
                    Id = Guid.NewGuid(),
                    UploadFileId = file.Id,
                    ResultJson = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        numericAnalyse = numericAnalyse,
                    }),

                };
            }

        }


        public async Task AnalyseExelFileAsync(Guid uploadFileId, CancellationToken cancellationToken)
        {
            var file = await _db.UploadFiles.FindAsync(new object[] { uploadFileId }, cancellationToken);
            if (file == null) return;

            using (var stream = new MemoryStream(file.Data))
            using (var workbook = new XLWorkbook(stream))
            {
                var worksheet = workbook.Worksheets.First();
                var usedRange = worksheet.RangeUsed();

                var numericValues = new List<double>();

                foreach (var row in usedRange.Rows())
                {
                    foreach (var cell in row.Cells())
                    {
                        if (double.TryParse(cell.Value.ToString(), out double val))
                        {
                            numericValues.Add(val);
                        }
                    }
                }

                if (numericValues.Count == 0) return;

                double sum = numericValues.Sum();
                double average = numericValues.Average();
                double min = numericValues.Min();
                double max = numericValues.Max();

                var exelAnalyse = new
                {
                    TotalNumbers = numericValues.Count,
                    Sum = sum,
                    Average = average,
                    Min = min,
                    Max = max,
                    Values = numericValues
                };

                var result = new AnalyseResult
                {
                    Id = Guid.NewGuid(),
                    UploadFileId = file.Id,
                    ResultJson = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        exelAnalyse = exelAnalyse,
                    }),
                    CreatedAtUtc = DateTime.UtcNow
                };

                _db.AnalyseResults.Add(result);
                await _db.SaveChangesAsync(cancellationToken);
            }

        }
    }
}
