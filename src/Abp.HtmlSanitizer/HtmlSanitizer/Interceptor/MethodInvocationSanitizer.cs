using System.Reflection;
using Abp;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Ganss.Xss;

namespace Abp.HtmlSanitizer.HtmlSanitizer.Interceptor
{
    /// <summary>
    /// This class is used to validate a method call (invocation) for method arguments.
    /// </summary>
    public class MethodInvocationSanitizer : ITransientDependency
    {
        private readonly IHtmlSanitizer _htmlSanitizer;
        protected MethodInfo Method { get; private set; }
        protected object[] ParameterValues { get; private set; }
        protected ParameterInfo[] Parameters { get; private set; }
        
        /// <summary>
        /// Creates a new <see cref="MethodInvocationSanitizer"/> instance.
        /// </summary>
        public MethodInvocationSanitizer(IHtmlSanitizer htmlSanitizer)
        {
            _htmlSanitizer = htmlSanitizer;
        }

        /// <param name="method">Method to be validated</param>
        /// <param name="parameterValues">List of arguments those are used to call the <paramref name="method"/>.</param>
        public virtual void Initialize(MethodInfo method, object?[] parameterValues)
        {
            Check.NotNull(method, nameof(method));
            Check.NotNull(parameterValues, nameof(parameterValues));

            Method = method;
            ParameterValues = parameterValues;
            Parameters = method.GetParameters();
        }

        /// <summary>
        /// Validates the method invocation.
        /// </summary>
        public void Sanitize()
        {
            CheckInitialized();

            if (Parameters.IsNullOrEmpty())
            {
                return;
            }

            if (!Method.IsPublic)
            {
                return;
            }
            
            if (Parameters.Length != ParameterValues.Length)
            {
                throw new Exception("Method parameter count does not match with argument count!");
            }

            for (var i = 0; i < Parameters.Length; i++)
            {
                SanitizeMethodParameter(Parameters[i], ParameterValues[i]);
            }

        }

        protected virtual void CheckInitialized()
        {
            if (Method == null)
            {
                throw new AbpException("This object has not been initialized. Call Initialize method first.");
            }
        }
        
        /// <summary>
        /// Validates given parameter for given value.
        /// </summary>
        /// <param name="parameterInfo">Parameter of the method to validate</param>
        /// <param name="parameterValue">Value to validate</param>
        protected virtual void SanitizeMethodParameter(ParameterInfo parameterInfo, object parameterValue)
        {
            
        }
        
    }
}
