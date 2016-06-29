using System;
using System.Collections.Generic;
using System.Reflection;

namespace Abp.Web.Api.Modeling
{
    [Serializable]
    public class ActionApiDescriptionModel
    {
        public MethodInfo Method
        {
            get { return _method; }
            set { _method = value; }
        }
        [NonSerialized]
        private MethodInfo _method;

        public string Name { get; }

        public string HttpMethod { get; }

        public string Url { get; }

        public IList<ParameterApiDescriptionModel> Parameters { get; }

        private ActionApiDescriptionModel()
        {

        }

        public ActionApiDescriptionModel(MethodInfo method, string name, string url, string httpMethod = null)
        {
            Method = method;
            Name = name;
            Url = url;
            HttpMethod = httpMethod;

            Parameters = new List<ParameterApiDescriptionModel>();
        }

        public ParameterApiDescriptionModel AddParameter(ParameterApiDescriptionModel parameter)
        {
            Parameters.Add(parameter);
            return parameter;
        }
    }
}