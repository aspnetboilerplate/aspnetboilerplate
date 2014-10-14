using System;
using Abp.Configuration.Startup;
using Abp.Dependency;

namespace Abp.Web.Models
{
    public class ErrorInfoBuilder : IErrorInfoBuilder, ISingletonDependency
    {
        public static IErrorInfoBuilder Instance { get { return IocManager.Instance.Resolve<IErrorInfoBuilder>(); } }

        private IExceptionToErrorInfoConverter Converter { get; set; }

        public ErrorInfoBuilder(IModuleConfigurations configuration)
        {
            Converter = new DefaultErrorInfoConverter(configuration);
        }

        public ErrorInfo BuildForException(Exception exception)
        {
            return Converter.Convert(exception);
        }

        /// <summary>
        /// Adds an exception converter that is used by <see cref="ForException"/> method.
        /// </summary>
        /// <param name="converter">Converter object</param>
        public void AddExceptionConverter(IExceptionToErrorInfoConverter converter)
        {
            converter.Next = Converter;
            Converter = converter;
        }
    }
}