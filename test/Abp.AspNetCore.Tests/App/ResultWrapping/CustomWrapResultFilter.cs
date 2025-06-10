using Abp.Web.Results.Filters;

namespace Abp.AspNetCore.App.ResultWrapping;

public class CustomWrapResultFilter : IWrapResultFilter
{
    public bool HasFilterForWrapOnSuccess(string url, out bool wrapOnSuccess)
    {
        wrapOnSuccess = false;
        return url.EndsWith("WrapResultTest/GetDontWrapByUrl");
    }

    public bool HasFilterForWrapOnError(string url, out bool wrapOnError)
    {
        wrapOnError = false;
        return url.EndsWith("WrapResultTest/GetDontWrapByUrlWithException");
    }
}