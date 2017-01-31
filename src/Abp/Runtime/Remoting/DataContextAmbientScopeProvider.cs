using System;
using System.Collections.Concurrent;
using Abp.Collections.Extensions;
using Abp.Domain.Uow;
using Castle.Core.Logging;
using JetBrains.Annotations;

namespace Abp.Runtime.Remoting
{
    /// <summary>
    /// CallContext implementation of <see cref="ICurrentUnitOfWorkProvider"/>. 
    /// This is the default implementation.
    /// </summary>
    public class DataContextAmbientScopeProvider<T> : IAmbientScopeProvider<T>
        where T : class
    {
        public ILogger Logger { get; set; }

        private static readonly ConcurrentDictionary<string, ScopeItem> ScopeDictionary = new ConcurrentDictionary<string, ScopeItem>();

        private readonly IAmbientDataContext _dataContext;

        public DataContextAmbientScopeProvider([NotNull] IAmbientDataContext dataContext)
        {
            Check.NotNull(dataContext, nameof(dataContext));

            _dataContext = dataContext;

            Logger = NullLogger.Instance;
        }

        public T GetValue(string contextKey)
        {
            return GetCurrentItem(contextKey)?.Value;
        }

        public IDisposable BeginScope(string contextKey, T value)
        {
            var item = new ScopeItem(value, GetCurrentItem(contextKey));

            if (!ScopeDictionary.TryAdd(item.Id, item))
            {
                throw new AbpException("Can not set unit of work! ScopeDictionary.TryAdd returns false!");
            }

            _dataContext.SetData(contextKey, item.Id);

            return new DisposeAction(() =>
            {
                ScopeDictionary.TryRemove(item.Id, out item);

                if (item.Outer == null)
                {
                    _dataContext.SetData(contextKey, null);
                    return;
                }

                _dataContext.SetData(contextKey, item.Outer.Id);
            });
        }

        private ScopeItem GetCurrentItem(string contextKey)
        {
            var objKey = _dataContext.GetData(contextKey) as string;
            return objKey != null ? ScopeDictionary.GetOrDefault(objKey) : null;
        }

        private class ScopeItem
        {
            public string Id { get; }

            public ScopeItem Outer { get; }

            public T Value { get; }

            public ScopeItem(T value, ScopeItem outer = null)
            {
                Id = Guid.NewGuid().ToString();

                Value = value;
                Outer = outer;
            }
        }
    }
}