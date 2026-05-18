using Minio;
using Minio.DataModel.Args;
using quiz_project.Interfaces;

namespace quiz_project.Services
{
    public class FileStorageService(IMinioClient minioClient, IConfiguration configuration) : IFileStorageService
    {
        private readonly string _bucket = configuration["R2:BucketName"]!;

        public async Task<string> UploadAsync(IFormFile file, int resourceId, string folder = "chapters")
        {
            var extension = Path.GetExtension(file.FileName);
            var objectKey = $"{folder}/{resourceId}/{Guid.NewGuid()}{extension}";

            using var stream = file.OpenReadStream();

            await minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_bucket)
                .WithObject(objectKey)
                .WithStreamData(stream)
                .WithObjectSize(file.Length)
                .WithContentType(file.ContentType));

            return objectKey;
        }

        public async Task<(Stream stream, string contentType, string fileName)> DownloadAsync(string objectKey)
        {
            var ms = new MemoryStream();

            await minioClient.GetObjectAsync(new GetObjectArgs()
                .WithBucket(_bucket)
                .WithObject(objectKey)
                .WithCallbackStream((stream, ct) =>
                {
                    stream.CopyToAsync(ms, ct).GetAwaiter().GetResult();
                    return Task.CompletedTask;
                }));

            ms.Position = 0;
            var fileName = Path.GetFileName(objectKey);
            return (ms, "application/octet-stream", fileName);
        }

        public async Task DeleteAsync(string objectKey)
        {
            await minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(_bucket)
                .WithObject(objectKey));
        }
    }
}
