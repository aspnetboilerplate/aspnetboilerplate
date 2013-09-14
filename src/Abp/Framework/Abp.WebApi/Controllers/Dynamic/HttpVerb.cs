using System;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace Abp.WebApi.Controllers.Dynamic
{
    [Flags]
    public enum HttpVerb
    {
        Get,
        Post,
        Put,
        Delete
    }
}