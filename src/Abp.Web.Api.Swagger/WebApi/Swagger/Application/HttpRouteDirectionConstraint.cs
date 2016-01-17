using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Routing;

namespace Abp.WebApi.Swagger.Application
{
    /// <summary>
    /// Implement IHttpRouteConstraint; Represents a base class route constraint
    /// </summary>
    public class HttpRouteDirectionConstraint : IHttpRouteConstraint
    {
        private readonly HttpRouteDirection _allowedDirection;

        public HttpRouteDirectionConstraint(HttpRouteDirection allowedDirection)
        {
            _allowedDirection = allowedDirection;
        }

        public bool Match(
            HttpRequestMessage request,
            IHttpRoute route,
            string parameterName,
            IDictionary<string, object> values,
            HttpRouteDirection routeDirection)
        {
            return routeDirection == _allowedDirection;
        }
    }
}
