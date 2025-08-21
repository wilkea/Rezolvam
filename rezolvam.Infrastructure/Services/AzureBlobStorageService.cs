using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using rezolvam.Application.Interfaces;

public class AzureBlobStorageService : IFileStorageService
{
    private readonly ILogger<AzureBlobStorageService> _logger;
    private readonly string _connectionString;
    private readonly string _containerName;

    public AzureBlobStorageService(ILogger<AzureBlobStorageService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration["AzureStorage:ConnectionString"]
            ?? throw new ArgumentNullException("AzureStorage:ConnectionString is not configured");
        _containerName = configuration["AzureStorage:ContainerName"]
            ?? throw new ArgumentNullException("AzureStorage:ContainerName is not configured");
    }

    public async Task<string> SaveFileAsync(IFormFile file, string pupupu)
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync();

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var blobClient = containerClient.GetBlobClient(fileName);

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);

        return blobClient.Uri.ToString();
    }

    public async Task<bool> DeleteFileAsync(string fileUrl, string pupupu)
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

        var fileName = Path.GetFileName(new Uri(fileUrl).LocalPath);
        var blobClient = containerClient.GetBlobClient(fileName);

        return await blobClient.DeleteIfExistsAsync();
    }

    public async Task<List<string>> SaveFilesAsync(List<IFormFile> files, string pupupu)
    {
        var urls = new List<string>();
        foreach (var file in files)
        {
            var url = await SaveFileAsync(file, _containerName);
            urls.Add(url);
        }
        return urls;
    }
}
