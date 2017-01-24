using System.Collections.Generic;

namespace Abp.AspNetCore.Mvc.Views
{
    public interface IEmbeddedViewsConfiguration
    {
        List<EmbeddedViewInfo> Sources { get; }
    }
}