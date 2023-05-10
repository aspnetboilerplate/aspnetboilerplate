using System;
using System.Collections.Generic;
using System.Reflection;
using Abp.Dependency;

namespace Abp.HtmlSanitizer;

public interface IHtmlSanitizerConfiguration : ISingletonDependency
{
    bool IsEnabledForGetRequests { get; set; }

    List<Func<MethodInfo, bool>> Selectors { get; set; }

    bool KeepChildNodes { get; set; }
}