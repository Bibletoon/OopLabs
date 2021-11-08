using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Accounts;
using Banks.Banks;
using Banks.Transactions;
using Banks.UI.Commands;
using Banks.UI.Queries;
using Banks.UI.Views;
using Terminal.Gui;

namespace Banks.UI.ViewModels
{
    public class TransactionsViewModel : ViewModel
    {
        private TransactionsView _view;
        private Bank _bank;
        private CentralBank _centralBank;

        public TransactionsViewModel(Bank bank, CentralBank centralBank)
        {
            _bank = bank;
            _centralBank = centralBank;
            _view = new TransactionsView(this);
            GetAccountsQuery = new BaseQuery<List<BankAccount>>(GetAccounts);
            GetTransactionsByGuid = new BaseParametrizedQuery<List<Transaction>, Guid>(GetAccountTransactions);
            RevertTransactionCommand = new BaseParametrizedCommand<Guid>(RevertTransaction);
        }

        public BaseQuery<List<BankAccount>> GetAccountsQuery { get; }
        public BaseParametrizedQuery<List<Transaction>, Guid> GetTransactionsByGuid { get; }
        public BaseParametrizedCommand<Guid> RevertTransactionCommand { get; }

        public override void Dispose()
        {
            _view.Dispose();
        }

        protected override void Init(Toplevel top)
        {
            _view.Init(top);
        }

        private List<BankAccount> GetAccounts()
        {
            return _bank.GetAccounts().Cast<BankAccount>().ToList();
        }

        private CommandResult RevertTransaction(Guid arg)
        {
            try
            {
                _centralBank.RevertTransaction(_bank.Id, arg);
                return CommandResult.Success();
            }
            catch (Exception e)
            {
                return CommandResult.Fail(e.Message);
            }
        }

        private List<Transaction> GetAccountTransactions(Guid arg)
        {
            return _bank.FindAccount(arg).GetTransactionsHistory();
        }
    }
}