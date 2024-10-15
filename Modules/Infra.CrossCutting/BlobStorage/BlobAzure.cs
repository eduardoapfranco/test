using Infra.CrossCutting.BlobStorage.Interfaces;
using Infra.CrossCutting.BlobStorage.Models;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Infra.CrossCutting.BlobStorage
{
    [ExcludeFromCodeCoverage]
    public class BlobAzure : IBlob
    {
        private CloudStorageAccount _storageAccount;

        public BlobAzure(string connectionString)
        {
            if (CloudStorageAccount.TryParse(connectionString, out _storageAccount))
            {

            }
            else
            {
                throw new Exception("connectionString invalid");
            }
        }

        public async Task<string> UploadFileAsync(Stream stream, string folderName, string fileName)
        {
            var key = Guid.NewGuid().ToString();

            return await UploadFileAsync(stream, folderName, fileName, key);
        }

        public async Task<string> UploadFileAsync(Stream stream, string folderName, string fileName, string blobId)
        {
            var key = blobId;
            var container = await GetContainerAsync(folderName);
            var reference = container.GetBlockBlobReference(key);


            if (!string.IsNullOrWhiteSpace(fileName))
            {
                reference.Properties.ContentType = MimeTypes.GetMimeType(fileName);
                reference.Properties.ContentDisposition = $"attachment; filename=\"{fileName}\"";
            }


            await reference.UploadFromStreamAsync(stream);
            return key;
        }

        public async Task<string> CopyFileAsync(string folderName, string newFolderName, string key)
        {
            var container = await GetContainerAsync(folderName);
            var reference = container.GetBlockBlobReference(key);


            var containerCopy = await GetContainerAsync(newFolderName);
            var newReference = containerCopy.GetBlockBlobReference(key);

            await reference.StartCopyAsync(newReference);
            return key;
        }

        public async Task<string> MoveFileAsync(string folderName, string newFolderName, string key)
        {
            await CopyFileAsync(folderName, newFolderName, key);
            await RemoveFileAsync(folderName, key);
            return key;
        }

        public async Task<string> RenameFileAsync(string folderName, string key, string newFileName)
        {
            var newKey = Guid.NewGuid().ToString();

            var container = await GetContainerAsync(folderName);
            var reference = container.GetBlockBlobReference(key);


            var containerCopy = await GetContainerAsync(folderName);
            var newReference = containerCopy.GetBlockBlobReference(newKey);

            await reference.StartCopyAsync(newReference);
            await RemoveFileAsync(folderName, key);

            return newKey;
        }

        public async Task RemoveFileAsync(string folderName, string key)
        {
            var container = await GetContainerAsync(folderName);
            var reference = container.GetBlockBlobReference(key);

            await reference.DeleteAsync();
        }


        public async Task<string> GenerateUrlAsync(string folderName, string key, int timeExpirationMinutes)
        {
            var container = await GetContainerAsync(folderName);
            var reference = container.GetBlockBlobReference(key);

            var shared = new SharedAccessBlobPolicy();

            shared.SharedAccessExpiryTime = DateTime.Now.AddMinutes(timeExpirationMinutes);
            shared.Permissions = SharedAccessBlobPermissions.Read;

            var sharedAcess = reference.GetSharedAccessSignature(shared);

            return reference.Uri + sharedAcess;
        }

        public async Task<ObjectDownload> DownloadFileAsync(string folderName, string key)
        {
            var container = await GetContainerAsync(folderName);
            var reference = container.GetBlockBlobReference(key);


            var stream = new MemoryStream();
            using (var ms = new MemoryStream())
            {
                await reference.DownloadToStreamAsync(ms);
                ms.Position = 0;
                ms.CopyTo(stream);
                stream.Position = 0;
                return new ObjectDownload(stream, reference.Properties.ContentDisposition, reference.Properties.ContentType);
            }
        }

        private async Task<CloudBlobContainer> GetContainerAsync(string containerName)
        {
            var containerReference = _storageAccount.CreateCloudBlobClient().GetContainerReference(containerName);

            await containerReference.CreateIfNotExistsAsync();
            await containerReference.SetPermissionsAsync(new BlobContainerPermissions()
            {
                PublicAccess = BlobContainerPublicAccessType.Off
            }, null, null, null);

            return containerReference;
        }

        public async Task<bool> BlobExistsAsync(string folderName, string key)
        {
            var container = await GetContainerAsync(folderName);
            var blobReference = container.GetBlobReference(key);
            return blobReference.Exists();
        }

        public async Task<ObjectDownload> ReadAsync(string folderName, string key)
        {
            var container = await GetContainerAsync(folderName);
            var reference = container.GetBlockBlobReference(key);

            return new ObjectDownload(await reference.OpenReadAsync(), reference.Properties.ContentDisposition, reference.Properties.ContentType);
        }
    }
}
