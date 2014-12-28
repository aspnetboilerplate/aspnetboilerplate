using System;
using System.Transactions;

namespace Abp.Domain.Uow
{
    internal class UnitOfWorkDefaultOptions : IUnitOfWorkDefaultOptions
    {
        /// <summary>
        /// Default: true.
        /// </summary>
        public bool IsTransactional { get; set; }

        public TimeSpan? Timeout { get; set; }

        public IsolationLevel? IsolationLevel { get; set; }

        public UnitOfWorkDefaultOptions()
        {
            IsTransactional = true;
        }
    }
}