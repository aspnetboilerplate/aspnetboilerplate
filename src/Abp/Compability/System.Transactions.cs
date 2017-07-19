namespace System.Transactions
{
#if !NET46
    public enum TransactionScopeOption
    {
        Required,
        RequiresNew,
        Suppress,
    }

    /// <summary>Specifies the isolation level of a transaction.</summary>
    public enum IsolationLevel
    {
        Serializable,
        RepeatableRead,
        ReadCommitted,
        ReadUncommitted,
        Snapshot,
        Chaos,
        Unspecified,
    }

    /// <summary>[Supported in the .NET Framework 4.5.1 and later versions] Specifies whether transaction flow across thread continuations is enabled for <see cref="T:System.Transactions.TransactionScope" />.</summary>
    public enum TransactionScopeAsyncFlowOption
    {
        Suppress,
        Enabled,
    }
#endif
}
