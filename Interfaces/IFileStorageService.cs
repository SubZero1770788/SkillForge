namespace quiz_project.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(IFormFile file, int chapterId);
        Task<string> GetPresignedUrlAsync(string objectKey);
        Task DeleteAsync(string objectKey);
    }
}
