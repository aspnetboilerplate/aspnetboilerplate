using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Abp.WebApi.Swagger.Adapter.Converter
{
    public static class JTokenExtension
    {
        public static JToken RemoveFields(this JToken token, params string[] fields)
        {
            var container = token as JContainer;
            if (container == null) return token;

            var removeList = new List<JToken>();
            foreach (var child in container.Children())
            {
                var prop = child as JProperty;
                if (prop != null && fields.Contains(prop.Name))
                {
                    removeList.Add(prop);
                }
                
                child.RemoveFields(fields);
            }

            foreach (var el in removeList)
            {
                el.Remove();
            }

            return token;
        }
    }
}
