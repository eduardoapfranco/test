using Domain.Input.Iugu;
using Infra.CrossCutting.UoW.Models;

namespace Application.AppServices.SignatureApplication.ViewModels
    {
    public class SignatureViewModel : BaseResult
        {
        public Signature IuguSignature { get; set; }
        }
    }
