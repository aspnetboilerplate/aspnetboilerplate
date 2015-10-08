using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace MyProject.News
{
    public interface INewsRepository:IRepository<News,long>
    {

    }
}
