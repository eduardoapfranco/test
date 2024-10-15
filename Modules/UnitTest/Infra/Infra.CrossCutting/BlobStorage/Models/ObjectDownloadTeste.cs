using Infra.CrossCutting.BlobStorage.Models;
using System.IO;
using Xunit;

namespace UnitTest.Infra.Infra.CrossCutting.BlobStorage.Models
{
    public class ObjectDownloadTeste
    {
        [Fact(DisplayName = "Should create object download")]
        [Trait("[Infra.CrossCutting]-BlobStorage", "CreateObjectDownload")]
        public void ShouldCreateObjectDownload()
        {
            using (var ms = new MemoryStream())
            {
                var download = new ObjectDownload(ms, "attachment; filename=\"testeDeExtensao.xlsx\"",
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                Assert.Equal(ms, download.StreamFile);
                Assert.Equal("attachment; filename=\"testeDeExtensao.xlsx\"", download.ContentDisposition);
                Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", download.ContentType);
                Assert.Equal("testeDeExtensao.xlsx", download.FileName);
                Assert.Equal(".xlsx", download.Extension);
            }
        }
    }
}
