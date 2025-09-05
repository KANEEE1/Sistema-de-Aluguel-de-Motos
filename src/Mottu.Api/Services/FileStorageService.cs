using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mottu.Api.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly ILogger<FileStorageService> _logger;
        private readonly string _basePath;

        public FileStorageService(ILogger<FileStorageService> logger)
        {
            _logger = logger;
            _basePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Arquivo inválido");

            if (!IsValidImageFormat(file))
                throw new ArgumentException("Formato de arquivo inválido. Apenas PNG e BMP são permitidos");

            var folderPath = Path.Combine(_basePath, folder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation("Arquivo salvo: {FilePath}", filePath);
            return Path.Combine(folder, fileName).Replace("\\", "/");
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_basePath, filePath);
                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                    _logger.LogInformation("Arquivo removido: {FilePath}", fullPath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover arquivo: {FilePath}", filePath);
                return false;
            }
        }

        public async Task<byte[]?> GetFileAsync(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_basePath, filePath);
                if (File.Exists(fullPath))
                {
                    return await File.ReadAllBytesAsync(fullPath);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao ler arquivo: {FilePath}", filePath);
                return null;
            }
        }

        public bool IsValidImageFormat(IFormFile file)
        {
            var allowedExtensions = new[] { ".png", ".bmp" };
            var allowedMimeTypes = new[] { "image/png", "image/bmp" };

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var mimeType = file.ContentType.ToLowerInvariant();

            return allowedExtensions.Contains(extension) && allowedMimeTypes.Contains(mimeType);
        }
    }
}
