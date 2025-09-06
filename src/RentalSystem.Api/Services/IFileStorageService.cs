namespace RentalSystem.Api.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string folder);
        Task<bool> DeleteFileAsync(string filePath);
        Task<byte[]?> GetFileAsync(string filePath);
        bool IsValidImageFormat(IFormFile file);
    }
}
