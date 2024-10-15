using Infra.CrossCutting.BlobStorage.Models;
using System.IO;
using System.Threading.Tasks;

namespace Infra.CrossCutting.BlobStorage.Interfaces
{
    public interface IBlob
    {
        Task<string> UploadFileAsync(Stream stream, string folderName, string fileName);
        Task<string> UploadFileAsync(Stream stream, string folderName, string fileName, string blobId);
        Task<string> CopyFileAsync(string folderName, string newFolderName, string key);
        Task<string> MoveFileAsync(string folderName, string newFolderName, string key);
        Task RemoveFileAsync(string folderName, string key);
        Task<string> GenerateUrlAsync(string folderName, string key, int timeExpirationMinutes);
        Task<ObjectDownload> DownloadFileAsync(string folderName, string key);
        Task<bool> BlobExistsAsync(string folderName, string key);
        Task<ObjectDownload> ReadAsync(string folderName, string key);
        Task<string> RenameFileAsync(string folderName, string key, string newFileName);
    }
}
