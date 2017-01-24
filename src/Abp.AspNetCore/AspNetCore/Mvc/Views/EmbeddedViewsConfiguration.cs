using System.Collections.Generic;

namespace Abp.AspNetCore.Mvc.Views
{
    public class EmbeddedViewsConfiguration : IEmbeddedViewsConfiguration
    {
        public List<EmbeddedViewInfo> Sources { get; }

        public EmbeddedViewsConfiguration()
        {
            Sources = new List<EmbeddedViewInfo>();
        }
    }
}