using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;

namespace Abp.Configuration
{
    /// <summary>
    /// Repository to manage setting records.
    /// </summary>
    public interface ISettingValueRepository : IRepository<SettingValueRecord, long>
    {

    }
}
