namespace Banks.Accounts
{
    public class FinalWrapper : AccountWrapperBase
    {
        public FinalWrapper(BankAccount account)
            : base(account)
        {
        }

        private FinalWrapper()
        {
        }
    }
}