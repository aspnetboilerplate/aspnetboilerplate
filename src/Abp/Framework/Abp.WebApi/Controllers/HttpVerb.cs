using System;

namespace Abp.WebApi.Controllers
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