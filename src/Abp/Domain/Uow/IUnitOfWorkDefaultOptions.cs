using System;
using System.Transactions;

namespace Abp.Domain.Uow
{
    public interface IUnitOfWorkDefaultOptions
    {
        /// <summary>
        /// Default: true.
        /// </summary>
        bool IsTransactional { get; set; }

        TimeSpan? Timeout { get; set; }

        IsolationLevel? IsolationLevel { get; set; }
    }
}