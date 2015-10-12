using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yishe.Abp.Handler.Throttle
{
    public class ThrottlingHandler
          : DelegatingHandler
    {

        private readonly TimeSpan _period;
        private readonly string _message;
        private InMemoryThrottleStore _store;
        private static long IpLimitCount = Convert.ToInt64(System.Configuration.ConfigurationManager.AppSettings["IpLimitCount"]);
        private static long UserLimitCount = Convert.ToInt64(System.Configuration.ConfigurationManager.AppSettings["UserLimitCount"]);
        private static long DefaultLimitCount = Convert.ToInt64(System.Configuration.ConfigurationManager.AppSettings["DefaultLimitCount"]);


        public ThrottlingHandler()
        {
            _store = InMemoryThrottleStore.GetInstence();
            _period = TimeSpan.FromMinutes(1);
            _message = "{\"code\":4007,\"Msg\":\"请求过于频繁\"}";
        }

        protected virtual string GetUserIdentifier(HttpRequestMessage request)
        {
            var user = request.GetRequestContext().Principal;
            if (user != null && !string.IsNullOrEmpty(user.Identity.Name))
            {
                return "User:" + user.Identity.Name;
            }
            if (request.Headers.Contains("X-Forward-For"))
            {

                var vaules = request.Headers.GetValues("X-Forward-For");
                string ip = "";
                foreach (var item in vaules)
                {
                    ip = item;

                }
                return "IP:" + ip;
            }
            else
            {
                return "未认证且未知IP ";
            };
        }
        public long GetMaxConts(string identifier)
        {
            if (identifier.StartsWith("IP:"))
            {
                return IpLimitCount;
            }
            else if (identifier.StartsWith("User:"))
            {
                return UserLimitCount;
            }
            else return DefaultLimitCount;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var identifier = GetUserIdentifier(request);

            if (string.IsNullOrEmpty(identifier))
            {
                return CreateResponse(request, HttpStatusCode.Forbidden, "Could not identify client.");
            }

            var maxRequests = GetMaxConts(identifier);

            ThrottleEntry entry = null;
            if (_store.TryGetValue(identifier, out entry))
            {
                if (entry.PeriodStart + _period < DateTime.UtcNow)
                {
                    _store.Rollover(identifier);
                }
            }
            _store.IncrementRequests(identifier);
            if (!_store.TryGetValue(identifier, out entry))
            {
                return CreateResponse(request, HttpStatusCode.Forbidden, "Could not identify client.");
            }

            Task<HttpResponseMessage> response = null;
            if (entry.Requests > maxRequests)
            {
                response = CreateResponse(request, HttpStatusCode.Conflict, _message);

            }
            else
            {
                response = base.SendAsync(request, cancellationToken);
            }

            return response.ContinueWith(task =>
            {
                var remaining = maxRequests - entry.Requests;
                if (remaining < 0)
                {
                    remaining = 0;
                }

                var httpResponse = task.Result;
                httpResponse.Headers.Add("RateLimit-Limit", maxRequests.ToString());
                httpResponse.Headers.Add("RateLimit-Remaining", remaining.ToString());

                return httpResponse;
            });
        }

        protected Task<HttpResponseMessage> CreateResponse(HttpRequestMessage request, HttpStatusCode statusCode, string message)
        {
            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            var response = request.CreateResponse(statusCode);

            response.Content = new StringContent(message, Encoding.GetEncoding("UTF-8"), "application/json");

            tsc.SetResult(response);
            return tsc.Task;
        }
    }
}
