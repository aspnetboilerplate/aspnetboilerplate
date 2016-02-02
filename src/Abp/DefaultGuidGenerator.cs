using System;
using Abp.Dependency;

namespace Abp
{
    /// <summary>
    /// Implements <see cref="IGuidGenerator"/> by using <see cref="Guid.NewGuid"/>.
    /// </summary>
    public class DefaultGuidGenerator : IGuidGenerator, ITransientDependency
    {
        public static DefaultGuidGenerator Instance { get { return _instance; } }
        private static readonly DefaultGuidGenerator _instance = new DefaultGuidGenerator();

        public virtual Guid Create()
        {
            return Guid.NewGuid();
        }
    }
}