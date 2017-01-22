using System;
using System.Linq;
using System.Reflection;
using System.Transactions;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// This attribute is used to indicate that declaring method is atomic and should be considered as a unit of work.
    /// A method that has this attribute is intercepted, a database connection is opened and a transaction is started before call the method.
    /// At the end of method call, transaction is committed and all changes applied to the database if there is no exception,
    /// otherwise it's rolled back. 
    /// </summary>
    /// <remarks>
    /// This attribute has no effect if there is already a unit of work before calling this method, if so, it uses the same transaction.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class UnitOfWorkAttribute : Attribute
    {
        /// <summary>
        /// Scope option.
        /// </summary>
        public TransactionScopeOption? Scope { get; set; }

        /// <summary>
        /// Is this UOW transactional?
        /// Uses default value if not supplied.
        /// </summary>
        public bool? IsTransactional { get; set; }

        /// <summary>
        /// Timeout of UOW As milliseconds.
        /// Uses default value if not supplied.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// If this UOW is transactional, this option indicated the isolation level of the transaction.
        /// Uses default value if not supplied.
        /// </summary>
        public IsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        /// Used to prevent starting a unit of work for the method.
        /// If there is already a started unit of work, this property is ignored.
        /// Default: false.
        /// </summary>
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Creates a new UnitOfWorkAttribute object.
        /// </summary>
        public UnitOfWorkAttribute()
        {

        }

        /// <summary>
        /// Creates a new <see cref="UnitOfWorkAttribute"/> object.
        /// </summary>
        /// <param name="isTransactional">
        /// Is this unit of work will be transactional?
        /// </param>
        public UnitOfWorkAttribute(bool isTransactional)
        {
            IsTransactional = isTransactional;
        }

        /// <summary>
        /// Creates a new <see cref="UnitOfWorkAttribute"/> object.
        /// </summary>
        /// <param name="timeout">As milliseconds</param>
        public UnitOfWorkAttribute(int timeout)
        {
            Timeout = TimeSpan.FromMilliseconds(timeout);
        }

        /// <summary>
        /// Creates a new <see cref="UnitOfWorkAttribute"/> object.
        /// </summary>
        /// <param name="isTransactional">Is this unit of work will be transactional?</param>
        /// <param name="timeout">As milliseconds</param>
        public UnitOfWorkAttribute(bool isTransactional, int timeout)
        {
            IsTransactional = isTransactional;
            Timeout = TimeSpan.FromMilliseconds(timeout);
        }

        /// <summary>
        /// Creates a new <see cref="UnitOfWorkAttribute"/> object.
        /// <see cref="IsTransactional"/> is automatically set to true.
        /// </summary>
        /// <param name="isolationLevel">Transaction isolation level</param>
        public UnitOfWorkAttribute(IsolationLevel isolationLevel)
        {
            IsTransactional = true;
            IsolationLevel = isolationLevel;
        }

        /// <summary>
        /// Creates a new <see cref="UnitOfWorkAttribute"/> object.
        /// <see cref="IsTransactional"/> is automatically set to true.
        /// </summary>
        /// <param name="isolationLevel">Transaction isolation level</param>
        /// <param name="timeout">Transaction  timeout as milliseconds</param>
        public UnitOfWorkAttribute(IsolationLevel isolationLevel, int timeout)
        {
            IsTransactional = true;
            IsolationLevel = isolationLevel;
            Timeout = TimeSpan.FromMilliseconds(timeout);
        }

        /// <summary>
        /// Creates a new <see cref="UnitOfWorkAttribute"/> object.
        /// <see cref="IsTransactional"/> is automatically set to true.
        /// </summary>
        /// <param name="scope">Transaction scope</param>
        public UnitOfWorkAttribute(TransactionScopeOption scope)
        {
            IsTransactional = true;
            Scope = scope;
        }

        /// <summary>
        /// Creates a new <see cref="UnitOfWorkAttribute"/> object.
        /// <see cref="IsTransactional"/> is automatically set to true.
        /// </summary>
        /// <param name="scope">Transaction scope</param>
        /// <param name="timeout">Transaction  timeout as milliseconds</param>
        public UnitOfWorkAttribute(TransactionScopeOption scope, int timeout)
        {
            IsTransactional = true;
            Scope = scope;
            Timeout = TimeSpan.FromMilliseconds(timeout);
        }

        /// <summary>
        /// Gets UnitOfWorkAttribute for given method or null if no attribute defined.
        /// </summary>
        /// <param name="methodInfo">Method to get attribute</param>
        /// <returns>The UnitOfWorkAttribute object</returns>
        internal static UnitOfWorkAttribute GetUnitOfWorkAttributeOrNull(MemberInfo methodInfo)
        {
            var attrs = methodInfo.GetCustomAttributes(true).OfType<UnitOfWorkAttribute>().ToArray();
            if (attrs.Length > 0)
            {
                return attrs[0];
            }

            if (UnitOfWorkHelper.IsConventionalUowClass(methodInfo.DeclaringType))
            {
                return new UnitOfWorkAttribute(); //Default
            }

            return null;
        }

        internal UnitOfWorkOptions CreateOptions()
        {
            return new UnitOfWorkOptions
            {
                IsTransactional = IsTransactional,
                IsolationLevel = IsolationLevel,
                Timeout = Timeout,
                Scope = Scope
            };
        }
    }
}