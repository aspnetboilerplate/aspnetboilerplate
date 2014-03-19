using Abp.Domain.Repositories.NHibernate;
using MySpaProject.People;

namespace MySpaProject.NHibernate.Repositories
{
    public class PersonRepository : NhRepositoryBase<Person>, IPersonRepository
    {

    }
}