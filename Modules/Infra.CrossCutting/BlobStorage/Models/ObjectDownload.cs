using System;
using System.IO;

namespace Infra.CrossCutting.BlobStorage.Models
{
    public class ObjectDownload
    {
        public Stream StreamFile { get; set; }
        public string ContentDisposition { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }

        public ObjectDownload(Stream streamFile, string contentDisposition, string contentType)
        {
            StreamFile = streamFile;
            ContentDisposition = contentDisposition;
            ContentType = contentType;

            if (!string.IsNullOrWhiteSpace(contentDisposition))
                FileName = contentDisposition
                    .Substring(contentDisposition.IndexOf("filename=", StringComparison.Ordinal) + 9, contentDisposition.Length - (contentDisposition.IndexOf("filename=", StringComparison.Ordinal) + 9))
                    .Replace("\"", "");

            if (!string.IsNullOrWhiteSpace(FileName))
                Extension = FileName.Substring(FileName.IndexOf(".", StringComparison.Ordinal), FileName.Length - FileName.IndexOf(".", StringComparison.Ordinal));
        }
    }
}
