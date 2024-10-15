using Application.AppServices.ContentSugestionApplication.Input;
using Application.AppServices.ContentSugestionApplication.ViewModel;
using System.Threading.Tasks;

namespace Application.Interfaces
    {
    public interface IContentSugestionApplication
        {
        Task<ContentSugestionViewModel> InsertAsync(ContentSugestionInput input);
        }
    }
