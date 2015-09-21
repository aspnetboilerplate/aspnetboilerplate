using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Domain.Repositories
{
    public interface IFromDB<TEntity> : IFromDB<TEntity, int> where TEntity : class, IEntity<int>
    {

    }
}
