using Abp.Runtime.Remoting;

namespace Abp.EntityHistory
{
    /// <summary>
    /// Implements null object pattern for <see cref="IEntityChangeSetReasonProvider"/>.
    /// </summary>
    public class NullEntityChangeSetReasonProvider : EntityChangeSetReasonProviderBase
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static NullEntityChangeSetReasonProvider Instance { get; } = new NullEntityChangeSetReasonProvider();

        /// <inheritdoc/>
        public override string Reason => null;

        private NullEntityChangeSetReasonProvider()
            : base(
                  new DataContextAmbientScopeProvider<ReasonOverride>(new AsyncLocalAmbientDataContext())
            )
        {

        }
    }
}
