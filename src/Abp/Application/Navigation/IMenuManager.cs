using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.Application.Navigation
{
    /// <summary>
    /// Manages <see cref="Menu"/>s in the application.
    /// </summary>
    public interface IMenuManager : ISingletonDependency
    {
    }
}
