using Application.AppServices.CategoryApplication.ViewModel;
using Application.Interfaces;
using Infra.CrossCutting.Controllers;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ConstruaApp.Api.Controllers
{
    [Route("api/v1")]
    [Authorize]
    [ApiController]
    public class CategoryController :  BaseController
    {

        private readonly ICategoryApplication _categoryApplication;

        public CategoryController(INotificationHandler<DomainNotification> notification, ICategoryApplication categoryApplication) : base(notification)
        {
            _categoryApplication = categoryApplication;
        }
        

        [HttpGet]
        [Route("categories-sync")]
        [ProducesResponseType(typeof(Result<IEnumerable<CategoryViewModel>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCategoriesLastDateUpdatedAsync([FromQuery] DateTime? lastDateSync)
        {
            return OkOrDefault(await _categoryApplication.GetCategoriesLastDateUpdatedAsync(lastDateSync));
        }

        [HttpGet]
        [Route("categories")]
        [ProducesResponseType(typeof(Result<IEnumerable<CategoryViewModel>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetRootCategoriesBasedOnProfileAsync()
        {
            int userId = (int)(int)GetUserLogged().Id;
            return OkOrDefault(await _categoryApplication.GetRootCategoriesBasedOnProfileAsync(userId));
        }

        [HttpGet]
        [Route("categories/parent/{categoryId}")]
        [ProducesResponseType(typeof(Result<IEnumerable<CategoryViewModel>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCategoriesByParentBasedOnProfileAsync([FromRoute] int categoryId)
        {
            return OkOrDefault(await _categoryApplication.GetCategoriesByParentBasedOnProfileAsync(categoryId));
        }

        [HttpGet]
        [Route("categories/all")]
        [ProducesResponseType(typeof(Result<IEnumerable<CategoryViewModel>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllAsync()
        {
            return OkOrDefault(await _categoryApplication.GetAllAsync());
        }
        
        [HttpGet]
        [Route("categories/{categoryId}")]
        [ProducesResponseType(typeof(Result<IEnumerable<CategoryViewModel>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SelectByIdAsync([FromRoute] int categoryId)
        {
            return OkOrDefault(await _categoryApplication.SelectByIdAsync(categoryId));
        }
    }
}