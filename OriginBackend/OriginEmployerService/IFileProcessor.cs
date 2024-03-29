namespace OriginEmployerService
{
    public interface IFileProcessor
    {
        Task ProcessFileAsync(string filePath, string employerName);
    }
}