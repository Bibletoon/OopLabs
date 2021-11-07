using System;
using Banks.Accounts.AccountConfigurations;

namespace Banks.Banks
{
    public class BankConfiguration
    {
        private Guid _id = Guid.NewGuid();

        public BankConfiguration(DepositAccountConfiguration depositAccountConfiguration, DebitAccountConfiguration debitAccountConfiguration, CreditAccountConfiguration creditAccountConfiguration, UnconfirmedClientLimits unconfirmedClientLimits)
        {
            ArgumentNullException.ThrowIfNull(debitAccountConfiguration);
            ArgumentNullException.ThrowIfNull(depositAccountConfiguration);
            ArgumentNullException.ThrowIfNull(creditAccountConfiguration);
            ArgumentNullException.ThrowIfNull(unconfirmedClientLimits);
            DepositAccountConfiguration = depositAccountConfiguration;
            DebitAccountConfiguration = debitAccountConfiguration;
            CreditAccountConfiguration = creditAccountConfiguration;
            UnconfirmedClientLimits = unconfirmedClientLimits;
        }

        private BankConfiguration()
        {
        }

        public DepositAccountConfiguration DepositAccountConfiguration { get; private set; }
        public DebitAccountConfiguration DebitAccountConfiguration { get; private set; }
        public CreditAccountConfiguration CreditAccountConfiguration { get; private set; }
        public UnconfirmedClientLimits UnconfirmedClientLimits { get; private set; }
    }
}