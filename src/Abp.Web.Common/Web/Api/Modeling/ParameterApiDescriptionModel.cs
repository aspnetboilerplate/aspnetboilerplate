using System;

namespace Abp.Web.Api.Modeling
{
    [Serializable]
    public class ParameterApiDescriptionModel
    {
        public string Name { get; }

        public string Type { get; }

        public bool IsOptional { get;  }

        public object DefaultValue { get;  }

        public string[] ConstraintTypes { get; }

        public string Source { get; }

        private ParameterApiDescriptionModel()
        {
            
        }

        public ParameterApiDescriptionModel(string name, string type, bool isOptional = false, object defaultValue = null, string[] constraintTypes = null, string source = null)
        {
            Name = name;
            Type = type;
            IsOptional = isOptional;
            DefaultValue = defaultValue;
            ConstraintTypes = constraintTypes;
            Source = source;
        }
    }
}