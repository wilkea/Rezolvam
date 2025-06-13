using Microsoft.AspNetCore.Http;

namespace rezolvam.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string containerName);
        Task<bool> DeleteFileAsync(string filePath, string containerName);
        Task<List<string>> SaveFilesAsync(List<IFormFile> files, string containerName);

    }
}
