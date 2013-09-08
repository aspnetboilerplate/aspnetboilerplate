using System;

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