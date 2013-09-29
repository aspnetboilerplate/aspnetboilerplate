using System;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// This attribute is used to indicate that declaring method is transactional (atomic).
    /// A method that has this attribute is intercepted, a transaction starts before call the method.
    /// At the end of method call, transaction is commited if there is no exception, othervise it's rolled back.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class UnitOfWorkAttribute : Attribute
    {

    }
}