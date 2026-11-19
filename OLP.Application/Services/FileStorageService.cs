using Microsoft.Extensions.Configuration;
using OLP.Application.Contracts.Services;
using OLP.Application.Exceptions;

namespace OLP.Application.Services
{
    public class FileStorageService : IFileStorageService
    {
        private const long MaxBytes = 10 * 1024 * 1024;
        private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase) { ".pdf", ".png", ".jpg", ".jpeg" };
        private readonly string _root;

        public FileStorageService(IConfiguration configuration)
        {
            _root = Path.GetFullPath(configuration["Files:Root"] ?? throw new InvalidOperationException("Configure Files:Root."));
        }

        public async Task<string> UploadAsync(Stream content, string fileName, CancellationToken cancellationToken = default)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (!Allowed.Contains(extension)) throw new BadRequestException("Unsupported file type.");
            Directory.CreateDirectory(_root);
            var key = Guid.NewGuid().ToString("N") + extension;
            var path = Resolve(key);
            try
            {
                await using var output = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, true);
                var buffer = new byte[81920];
                long total = 0;
                int read;
                while ((read = await content.ReadAsync(buffer.AsMemory(), cancellationToken)) != 0)
                {
                    total += read;
                    if (total > MaxBytes) throw new BadRequestException("File exceeds 10 MB.");
                    await output.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
                }
                if (total == 0) throw new BadRequestException("Empty file.");
            }
            catch { File.Delete(path); throw; }
            return key;
        }

        public Task DeleteAsync(string key) { File.Delete(Resolve(key)); return Task.CompletedTask; }

        public Task<Stream> OpenReadAsync(string key)
        {
            var path = Resolve(key);
            if (!File.Exists(path)) throw new NotFoundException("File not found.");
            return Task.FromResult<Stream>(File.OpenRead(path));
        }


        private string Resolve(string key)
        {
            if (key != Path.GetFileName(key) || !Allowed.Contains(Path.GetExtension(key)) ||
                !Guid.TryParseExact(Path.GetFileNameWithoutExtension(key), "N", out _))
                throw new BadRequestException("Invalid file key.");
            return Path.Combine(_root, key);
        }
    }
}
