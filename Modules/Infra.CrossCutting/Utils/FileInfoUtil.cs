using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Infra.CrossCutting.Utils
{
    [ExcludeFromCodeCoverage]
    public static class FileInfoUtil
    {
        public static long GetFileSize(string filePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                return -1;
            }
            else if (File.Exists(filePath))
            {
                return new FileInfo(filePath).Length;
            }
            return 0;
        }

        public static string GetFileExtension(string filePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                return "";
            }
            else if (File.Exists(filePath))
            {
                return Path.GetExtension(filePath);
            }
            return "";
        }

        public static string GetFileMimeType(string filePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                return "";
            }
            else if (File.Exists(filePath))
            {
                return MimeTypes.GetMimeType(filePath);
            }
            return "";
        }

        public static Stream GetFileStream(string filePath)
        {
            byte[] file = File.ReadAllBytes(filePath);
            Stream stream = new MemoryStream(file);
            return stream;
        }        
    }
}
