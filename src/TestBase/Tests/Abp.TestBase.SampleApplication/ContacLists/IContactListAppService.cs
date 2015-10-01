using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;

namespace Abp.TestBase.SampleApplication.ContacLists
{
    public interface IContactListAppService : IApplicationService
    {
        void Test();
    }
}
