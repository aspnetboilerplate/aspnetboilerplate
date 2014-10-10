using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Configuration.Startup;
using Abp.Dependency;

namespace Abp.Application.Navigation
{
    public interface INavigationProvider : ITransientDependency
    {
    }
}
