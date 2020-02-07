using System.Threading;
using Abp.Dependency;
using Abp.Threading;
using Microsoft.AspNetCore.Http;

namespace Abp.AspNetCore.Http.Threading
{
    public class HttpContextCancellationTokenProvider : ICancellationTokenProvider, ITransientDependency
    {
        public CancellationToken Token => _httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextCancellationTokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    }
}
