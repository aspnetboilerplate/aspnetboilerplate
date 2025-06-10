using Abp.AspNetCore.Mvc.Controllers;
using Abp.UI;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.App.Controllers;

public class WrapResultTestController : AbpController
{
    [HttpGet]
    [Route("WrapResultTest/Get")]
    public int Get()
    {
        return 42;
    }

    [HttpGet]
    [Route("WrapResultTest/GetDontWrap")]
    [DontWrapResult]
    public int GetDontWrap()
    {
        return 42;
    }

    [HttpGet]
    [Route("WrapResultTest/GetXml")]
    [Produces("application/xml")]
    public int GetXml()
    {
        return 42;
    }

    [HttpGet]
    [Route("WrapResultTest/GetDontWrapByUrl")]
    public int GetDontWrapByUrl()
    {
        return 42;
    }

    [HttpGet]
    [Route("WrapResultTest/GetDontWrapByUrlWithException")]
    public int GetDontWrapByUrlWithException()
    {
        throw new UserFriendlyException("Test error");
    }
}