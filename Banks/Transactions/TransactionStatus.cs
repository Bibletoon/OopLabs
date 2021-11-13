namespace Banks.Transactions
{
    public enum TransactionStatus
    {
#pragma warning disable SA1602
        Created,
        Completed,
        Failed,
        Canceled,
#pragma warning restore SA1602
    }
}