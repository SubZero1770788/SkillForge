namespace quiz_project.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(IFormFile file, int resourceId, string folder = "chapters");
        Task<(Stream stream, string contentType, string fileName)> DownloadAsync(string objectKey);
        Task DeleteAsync(string objectKey);
    }
}
