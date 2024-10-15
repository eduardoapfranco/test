using Infra.CrossCutting.Repository;
using System;

namespace Domain.Entities
{
    public class FileBlobStorage : BaseEntityDates<long>
    {
        public FileBlobStorage(string title, string blobId, string size, string type, string origin, bool zip = false)
        {
            Title = title;
            BlobId = blobId;
            Size = size;
            Type = type;
            Origin = origin;
            Zip = zip;
        }

        public FileBlobStorage()
        {}

        public string Title { get; set; }
        public string BlobId { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Origin { get; set; }
        public bool Zip { get; set; }

    }
}
