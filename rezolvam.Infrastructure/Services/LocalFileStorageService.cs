using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using rezolvam.Application.Interfaces;
using System.Text.RegularExpressions;

namespace rezolvam.Infrastructure.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly ILogger<LocalFileStorageService> _logger;
        private readonly string _rootPath;

        public LocalFileStorageService(ILogger<LocalFileStorageService> logger)
        {
            _logger = logger;
            // Store files in wwwroot/uploads
            _rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        public async Task<string> SaveFileAsync(IFormFile file, string containerName)
        {
            try
            {
                var extension = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{extension}";
                var containerPath = Path.Combine(_rootPath, containerName);

                // Create container directory if it doesn't exist
                if (!Directory.Exists(containerPath))
                {
                    Directory.CreateDirectory(containerPath);
                }

                var filePath = Path.Combine(containerPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return relative URL path
                return $"/{containerName}/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file {FileName}", file.FileName);
                throw;
            }
        }

        public Task<bool> DeleteFileAsync(string filePath, string containerName)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return Task.FromResult(false);
                }

                // Clean up the file path to prevent directory traversal
                filePath = filePath.TrimStart('/');
                var fullPath = Path.Combine(_rootPath, filePath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file {FilePath}", filePath);
                return Task.FromResult(false);
            }
        }
        public async Task<List<string>> SaveFilesAsync(List<IFormFile> files, string containerName)
        {
            var urls = new List<string>();

            foreach (var file in files)
            {
                var url = await SaveFileAsync(file, containerName);
                urls.Add(url);
            }

            return urls;
        }

    }
}
