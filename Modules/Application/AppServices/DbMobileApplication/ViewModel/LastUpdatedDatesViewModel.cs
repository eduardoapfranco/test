using Infra.CrossCutting.UoW.Models;
using System;

namespace Application.AppServices.DbMobileApplication.ViewModel
    {
    public class LastUpdatedDatesViewModel : BaseResult
        {
            public DateTime LastDateChecklist { get; set; }
            public DateTime LastDateCategory { get; set; }

        }
    }
