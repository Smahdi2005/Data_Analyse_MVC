using Data_Analyse_MVC.Models;

namespace Data_Analyse_MVC.Services
{
    public interface IFileAnalyseService
    {
        Task<AnalyseResult?> AnalyseTextFileAsync(Guid uploadFileId, CancellationToken cancellationToken);
        Task<AnalyseResult> AnalyseExelFileAsync(Guid uploadFileId, CancellationToken cancellationToken);

    }
}
