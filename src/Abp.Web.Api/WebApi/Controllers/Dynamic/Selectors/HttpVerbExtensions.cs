using System;
using System.Net.Http;
using Abp.Web;

namespace Abp.WebApi.Controllers.Dynamic.Selectors
{
    /// <summary>
    /// Extension methods for <see cref="HttpVerb"/>.
    /// </summary>
    public static class HttpVerbExtensions
    {
        public static readonly HttpMethod HttpPatch = new HttpMethod("PATCH");

        public static HttpMethod ToHttpMethod(this HttpVerb verb)
        {
            switch (verb)
            {
                case HttpVerb.Get:
                    return HttpMethod.Get;
                case HttpVerb.Post:
                    return HttpMethod.Post;
                case HttpVerb.Put:
                    return HttpMethod.Put;
                case HttpVerb.Delete:
                    return HttpMethod.Delete;
                case HttpVerb.Options:
                    return HttpMethod.Options;
                case HttpVerb.Trace:
                    return HttpMethod.Trace;
                case HttpVerb.Head:
                    return HttpMethod.Head;
                case HttpVerb.Patch:
                    return HttpPatch;
                default:
                    throw new ArgumentException("Given HttpVerb is unknown: " + verb, nameof(verb));
            }
        }

        public static HttpVerb ToHttpVerb(this HttpMethod method)
        {
            return HttpVerbHelper.Create(method.Method);
        }
    }
}