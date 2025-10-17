namespace Data_Analyse_MVC.Services
{
    public interface IFileAnalyseService
    {
        Task AnalyseTextFileAsync(Guid uploadFileId, CancellationToken cancellationToken);
        Task AnalyseExelFileAsync(Guid uploadFileId, CancellationToken cancellationToken);

    }
}
