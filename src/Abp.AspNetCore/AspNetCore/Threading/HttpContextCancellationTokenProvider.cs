using Abp.Dependency;
using Abp.Threading;
using Microsoft.AspNetCore.Http;
using System.Threading;

namespace Abp.AspNetCore.Threading
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
