using Minio;
using Minio.DataModel.Args;
using quiz_project.Interfaces;

namespace quiz_project.Services
{
    public class FileStorageService(IMinioClient minioClient, IConfiguration configuration) : IFileStorageService
    {
        private readonly string _bucket = configuration["R2:BucketName"]!;

        public async Task<string> UploadAsync(IFormFile file, int chapterId)
        {
            var extension = Path.GetExtension(file.FileName);
            var objectKey = $"chapters/{chapterId}/{Guid.NewGuid()}{extension}";

            using var stream = file.OpenReadStream();

            await minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_bucket)
                .WithObject(objectKey)
                .WithStreamData(stream)
                .WithObjectSize(file.Length)
                .WithContentType(file.ContentType));

            return objectKey;
        }

        public async Task<string> GetPresignedUrlAsync(string objectKey)
        {
            return await minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs()
                .WithBucket(_bucket)
                .WithObject(objectKey)
                .WithExpiry(60 * 60)); // 1 hour
        }

        public async Task DeleteAsync(string objectKey)
        {
            await minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(_bucket)
                .WithObject(objectKey));
        }
    }
}
