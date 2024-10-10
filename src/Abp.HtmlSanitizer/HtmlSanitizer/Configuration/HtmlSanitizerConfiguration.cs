using System;
using System.Collections.Generic;
using System.Reflection;

namespace Abp.HtmlSanitizer.Configuration;

public class HtmlSanitizerConfiguration : IHtmlSanitizerConfiguration
{
    public bool IsEnabledForGetRequests { get; set; }

    public bool KeepChildNodes { get; set; }

    public List<Func<MethodInfo, bool>> Selectors { get; set; }

    public HtmlSanitizerConfiguration()
    {
        IsEnabledForGetRequests = false;
        KeepChildNodes = false;
        Selectors = new List<Func<MethodInfo, bool>>();
    }
}