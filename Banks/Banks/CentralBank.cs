using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Accounts;
using Banks.Builders;
using Banks.Data;

namespace Banks.Banks
{
    public class CentralBank
    {
        private BanksDbContext _dbContext;
        private List<Bank> _banks;

        public CentralBank(BanksDbContext dbContext)
        {
            ArgumentNullException.ThrowIfNull(dbContext);
            _dbContext = dbContext;
            _dbContext.SetCentralBank(this);
            _dbContext.LoadData();
            _banks = _dbContext.Banks.ToList();
        }

        public Bank CreateBank(BankConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            var bank = new Bank(_dbContext, configuration);
            _banks.Add(bank);
            _dbContext.Banks.Add(bank);
            _dbContext.SaveChanges();
            return bank;
        }

        public void NotifyAboutFeePayment()
        {
            _banks.ForEach(b => b.PayFees());
        }

        internal BankAccount FindAccount(Guid id)
        {
            return _banks.Select(bank => bank.FindAccount(id)).FirstOrDefault(account => account is not null);
        }
    }
}