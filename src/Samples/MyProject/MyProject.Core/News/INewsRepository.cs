using Abp.Domain.Repositories;
using System.Collections.Generic;

namespace MyProject.News
{
    public interface INewsRepository:IRepository<News,long>
    {

    }
}
