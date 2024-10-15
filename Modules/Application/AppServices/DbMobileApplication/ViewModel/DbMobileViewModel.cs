using Infra.CrossCutting.UoW.Models;

namespace Application.AppServices.DbMobileApplication.ViewModel
{
    public class DbMobileViewModel : BaseResult
    {
        public string DownloadUrl { get; set; }
        public string DownloadUrlZip { get; set; }
    }
}
