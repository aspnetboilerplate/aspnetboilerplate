using System;

namespace Abp.Web.Api.Modeling
{
    [Serializable]
    public class ParameterApiDescriptionModel
    {
        public string Name { get; }

        public Type Type { get; }

        public string TypeAsString { get; }

        public bool IsOptional { get;  }

        public object DefaultValue { get;  }

        public string[] ConstraintTypes { get; }

        public string Source { get; }

        private ParameterApiDescriptionModel()
        {
            
        }

        public ParameterApiDescriptionModel(string name, Type type, bool isOptional = false, object defaultValue = null, string[] constraintTypes = null, string source = null)
        {
            Name = name;
            Type = type;
            TypeAsString = type.FullName;
            IsOptional = isOptional;
            DefaultValue = defaultValue;
            ConstraintTypes = constraintTypes;
            Source = source;
        }
    }
}