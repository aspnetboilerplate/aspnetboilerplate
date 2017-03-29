using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Abp.Collections.Extensions;

namespace Abp.Runtime.Validation
{
    [Serializable]
    public abstract class ValueValidatorBase : IValueValidator
    {
        public virtual string Name
        {
            get
            {
                var type = GetType().GetTypeInfo();
                if (type.IsDefined(typeof(ValidatorAttribute)))
                {
                    return type.GetCustomAttributes(typeof(ValidatorAttribute)).Cast<ValidatorAttribute>().First().Name;
                }

                return type.Name;
            }
        }

        /// <summary>
        /// Gets/sets arbitrary objects related to this object.
        /// Gets null if given key does not exists.
        /// </summary>
        /// <param name="key">Key</param>
        public object this[string key]
        {
            get { return Attributes.GetOrDefault(key); }
            set { Attributes[key] = value; }
        }

        /// <summary>
        /// Arbitrary objects related to this object.
        /// </summary>
        public IDictionary<string, object> Attributes { get; private set; }

        public abstract bool IsValid(object value);

        protected ValueValidatorBase()
        {
            Attributes = new Dictionary<string, object>();
        }
    }
}