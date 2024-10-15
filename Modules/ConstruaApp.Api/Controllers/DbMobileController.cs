using Application.AppServices.DbMobileApplication.Input;
using Application.AppServices.DbMobileApplication.ViewModel;
using Application.Interfaces;
using Infra.CrossCutting.Controllers;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ConstruaApp.Api.Controllers
    {
    [Route("api/v1/db-mobile")]
    [Authorize]
    [ApiController]
    public class DbMobileController : BaseController
    {

        private readonly IDbMobileApplication _dbMobileApplication;
        private IWebHostEnvironment _hostingEnvironment;
        private IConfiguration _configuration;

        public DbMobileController(INotificationHandler<DomainNotification> notification, IDbMobileApplication dbMobileApplication, IWebHostEnvironment environment, IConfiguration configuration) : base(notification)
        {
            _dbMobileApplication = dbMobileApplication;
            _hostingEnvironment = environment;
            _configuration = configuration;
        }


        [HttpPost]
        [Route("generate")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(DbMobileViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody]DbMobileInput input)
        {
            var path = Path.Combine(_hostingEnvironment.ContentRootPath, "construa.bd");
            return OkOrDefault(await _dbMobileApplication.CreateDBMobileAsync(input, path, _configuration.GetConnectionString("FolderDbMobile"), _hostingEnvironment.ContentRootPath));
        }


        //[HttpPost]
        //[ProducesResponseType(typeof(string), 201)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(500)]
        //[RequestSizeLimit(1073741824)]
        //[Route("upload-file")]
        //public async Task<IActionResult> UploadAsync([FromForm(Name = "file")]IFormFile file)
        //{
        //    using (var stream = file.OpenReadStream())
        //    {
        //        var blobId = await _blob.UploadFileAsync(stream, "db-mobile", file.FileName);
        //        return OkOrDefault(blobId);
        //    }
        //}

        //[HttpPost]
        //[ProducesResponseType(typeof(string), 201)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(500)]
        //[RequestSizeLimit(1073741824)]
        //[Route("upload-file2")]
        //public async Task<IActionResult> Upload2Async()
        //{
        //    var path = Path.Combine(_hostingEnvironment.ContentRootPath, "Db\\construa.bd");
        //    byte[] file = System.IO.File.ReadAllBytes(path);
        //    Stream stream = new MemoryStream(file);
        //    var blobId = await _blob.UploadFileAsync(stream, "db-mobile", "construa.bd");
        //    return OkOrDefault(blobId);
        //}

        [HttpGet]
        [ProducesResponseType(typeof(DbMobileViewModel), 200)]
        [ProducesResponseType(500)]
        [RequestSizeLimit(500000)]
        [Route("download")]
        public async Task<IActionResult> GetDownloadUrlAsync()
        {
            var folder = _configuration.GetSection("ConnectionStrings:FolderDbMobile").Value;
            var url = await _dbMobileApplication.GetLastDBMobileGeneratedAsync(folder);
            return OkOrDefault(url);
        }

        [HttpGet]
        [Route("last-updated-dates")]
        [ProducesResponseType(typeof(LastUpdatedDatesViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetLastUpdatedDatesAsync()
        {
            return OkOrDefault(await _dbMobileApplication.GetLastUpdatedDatesAsync());
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("check-env")]
        [ProducesResponseType(typeof(LastUpdatedDatesViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetEnv()
        {
            var obj = new string[] {"3_DSV_CONSTRUA_FOUR_" + _configuration.GetSection("EnvTest").Value };
            return OkOrDefault(obj);
        }
    }
 }