using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Abp.AspNetCore.Mvc.ExceptionHandling;

internal static class ClientCancellationHelper
{
    /// <summary>
    /// Checks if the given exception is thrown because the client has disconnected before the
    /// request was completed. Such requests are not application errors: ASP.NET Core reports them
    /// as <see cref="StatusCodes.Status499ClientClosedRequest"/> and logs them as debug messages.
    /// This check is intentionally the same as the one in ASP.NET Core's ExceptionHandlerMiddleware
    /// and Kestrel's HttpProtocol.
    /// </summary>
    public static bool IsClientCancellation(HttpContext httpContext, Exception exception)
    {
        return (exception is OperationCanceledException || exception is IOException) &&
               httpContext != null &&
               httpContext.RequestAborted.IsCancellationRequested;
    }
}
