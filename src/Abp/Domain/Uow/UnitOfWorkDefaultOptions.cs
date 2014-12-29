using System;
using System.Transactions;

namespace Abp.Domain.Uow
{
    internal class UnitOfWorkDefaultOptions : IUnitOfWorkDefaultOptions
    {
        /// <inheritdoc/>
        public bool IsTransactional { get; set; }

        /// <inheritdoc/>
        public TimeSpan? Timeout { get; set; }

        /// <inheritdoc/>
        public IsolationLevel? IsolationLevel { get; set; }

        public UnitOfWorkDefaultOptions()
        {
            IsTransactional = true;
        }
    }
}