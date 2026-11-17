
namespace OLP.Application.Contracts.Services
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(Stream content, string fileName, CancellationToken cancellationToken = default);
        Task<Stream> OpenReadAsync(string key);
        Task DeleteAsync(string key);
    }
}
