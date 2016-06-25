using System;
using System.Collections.Generic;

namespace Abp.Web.Api.Modeling
{
    [Serializable]
    public class ActionApiDescriptionModel
    {
        public string Name { get; }

        public string HttpMethod { get; }

        public string Url { get; }

        public IList<ParameterApiDescriptionModel> Parameters { get; }

        private ActionApiDescriptionModel()
        {

        }

        public ActionApiDescriptionModel(string name, string httpMethod, string url)
        {
            Name = name;
            HttpMethod = httpMethod;
            Url = url;

            Parameters = new List<ParameterApiDescriptionModel>();
        }

        public ParameterApiDescriptionModel AddParameter(ParameterApiDescriptionModel parameter)
        {
            Parameters.Add(parameter);
            return parameter;
        }
    }
}