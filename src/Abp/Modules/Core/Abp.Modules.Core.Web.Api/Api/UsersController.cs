using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Abp.Modules.Core.Application.Services;
using Abp.Modules.Core.Application.Services.Dto;
using Abp.WebApi.Controllers;

namespace Abp.Modules.Core.Api
{
    public class UsersController : AbpApiController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        public HttpResponseMessage Get()
        {
            throw new Exception("Heyoooo!");

            //var message = string.Format("Product with id = {0} not found", 3);
            //HttpError err = new HttpError(message);
            //return Request.CreateResponse(HttpStatusCode.NotFound, err);

            //return Request.CreateResponse(HttpStatusCode.OK, new { deneme = "yanılma!" });
            //validation
            //throwing appropriate messages
            //logging
            //exception handling
            //Logger.Info(L("GetAllQuestions_Method_Is_Called"));
            //return _userService.GetAllUsers();
            //return 
        }

        public int TestMethod(int a)
        {
            return 5 + a;
        }
    }
}
