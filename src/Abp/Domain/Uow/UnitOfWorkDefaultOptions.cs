using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;
using Abp.Application.Services;
using Abp.Domain.Repositories;

namespace Abp.Domain.Uow
{
    public class UnitOfWorkDefaultOptions : IUnitOfWorkDefaultOptions
    {
        public static List<Func<Type, bool>> ConventionalUowSelectorList = new List<Func<Type, bool>>
        {
            type => typeof(IRepository).IsAssignableFrom(type) ||
                    typeof(IApplicationService).IsAssignableFrom(type)
        };
        
        public TransactionScopeOption Scope { get; set; }

        /// <inheritdoc/>
        public bool IsTransactional { get; set; }

        /// <inheritdoc/>
        public TimeSpan? Timeout { get; set; }

        /// <inheritdoc/>
        public bool IsTransactionScopeAvailable { get; set; }

        /// <inheritdoc/>
        public IsolationLevel? IsolationLevel { get; set; }

        public IReadOnlyList<DataFilterConfiguration> Filters => _filters;
        
        private readonly List<DataFilterConfiguration> _filters;
        
        public IReadOnlyList<AuditFieldConfiguration> AuditFieldConfiguration => _auditFieldConfiguration;
        
        private readonly List<AuditFieldConfiguration> _auditFieldConfiguration;

        public List<Func<Type, bool>> ConventionalUowSelectors { get; }

        public UnitOfWorkDefaultOptions()
        {
            _filters = new List<DataFilterConfiguration>();
            _auditFieldConfiguration = new List<AuditFieldConfiguration>();
            IsTransactional = true;
            Scope = TransactionScopeOption.Required;

            IsTransactionScopeAvailable = true;

            ConventionalUowSelectors = ConventionalUowSelectorList.ToList();
        }

        public void RegisterFilter(string filterName, bool isEnabledByDefault)
        {
            if (_filters.Any(f => f.FilterName == filterName))
            {
                throw new AbpException("There is already a filter with name: " + filterName);
            }

            _filters.Add(new DataFilterConfiguration(filterName, isEnabledByDefault));
        }
        
        public void RegisterAuditFieldConfiguration(string fieldName, bool isSavingEnabledByDefault)
        {
            if (_auditFieldConfiguration.Any(f => f.FieldName == fieldName))
            {
                throw new AbpException("There is already a audit field configuration with name: " + fieldName);
            }

            _auditFieldConfiguration.Add(new AuditFieldConfiguration(fieldName, isSavingEnabledByDefault));
        }

        public void OverrideFilter(string filterName, bool isEnabledByDefault)
        {
            _filters.RemoveAll(f => f.FilterName == filterName);
            _filters.Add(new DataFilterConfiguration(filterName, isEnabledByDefault));
        }
    }
}
