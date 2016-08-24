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
        /// <summary>
        /// Compares a <see cref="HttpVerb"/> with a <see cref="HttpMethod"/>.
        /// </summary>
        /// <param name="verb">The Http verb</param>
        /// <param name="method">The Http method</param>
        /// <returns>True, if they are equal</returns>
        public static bool IsEqualTo(this HttpVerb verb, HttpMethod method)
        {
            return verb.ToHttpMethod() == method;
        }

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
                default:
                    throw new ArgumentException("Given HttpVerb is unknown: " + verb, nameof(verb));
            }
        }

        public static HttpVerb ToHttpVerb(this HttpMethod method)
        {
            if (method == HttpMethod.Get)
            {
                return HttpVerb.Get;
            }

            if (method == HttpMethod.Post)
            {
                return HttpVerb.Post;
            }

            if (method == HttpMethod.Put)
            {
                return HttpVerb.Put;
            }

            if (method == HttpMethod.Delete)
            {
                return HttpVerb.Delete;
            }

            if (method == HttpMethod.Options)
            {
                return HttpVerb.Options;
            }

            if (method == HttpMethod.Trace)
            {
                return HttpVerb.Trace;
            }

            if (method == HttpMethod.Head)
            {
                return HttpVerb.Head;
            }

            throw new ArgumentException("Given HttpMethod is unknown: " + method, nameof(method));
        }
    }
}