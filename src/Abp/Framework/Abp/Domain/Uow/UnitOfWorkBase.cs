using System;
using System.Collections.Generic;
using Abp.Utils.Extensions.Collections;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// Base for UnitOfWork classes.
    /// </summary>
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        private readonly List<Action> _successHandlers;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected UnitOfWorkBase()
        {
            _successHandlers = new List<Action>();
        }

        public abstract void Dispose();

        public abstract void Begin(bool isTransactional);

        public abstract void End();

        public abstract void Cancel();

        public virtual void AddSuccessHandler(Action action)
        {
            _successHandlers.Add(action);
        }

        /// <summary>
        /// Calls all success handlers.
        /// This method must be called if and only if unit of work success.
        /// </summary>
        /// <exception cref="AggregateException">Throws any of handlers throws exception</exception>
        protected void TriggerSuccessHandlers()
        {
            if (_successHandlers.IsNullOrEmpty())
            {
                return;
            }

            var exceptions = new List<Exception>();

            foreach (var successHandler in _successHandlers)
            {
                try
                {
                    successHandler();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException("There are exceptions in success handlers of unit of work", exceptions);
            }
        }
    }
}