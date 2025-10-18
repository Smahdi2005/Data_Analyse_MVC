using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data_Analyse_MVC.Models
{
    public class AnalyseResult
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UploadFileId { get; set; }

        [ForeignKey("UploadFileId")]
        public UploadFile UploadFile { get; set; } = default!;

        [Required]
        public string ResultJson { get; set; } = default!;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}

