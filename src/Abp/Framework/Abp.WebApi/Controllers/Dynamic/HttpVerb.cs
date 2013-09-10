using System;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace Abp.WebApi.Controllers.Dynamic
{
    [Flags]
    internal enum HttpVerb
    {
        Get,
        Post,
        Put,
        Delete
    }
}