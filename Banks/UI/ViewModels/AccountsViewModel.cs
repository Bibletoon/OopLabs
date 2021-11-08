using System.Collections.Generic;
using System.Linq;
using Banks.Accounts;
using Banks.Banks;
using Banks.Clients;
using Banks.Data;
using Banks.Transactions;
using Banks.UI.Commands;
using Banks.UI.Commands.CommandArguments;
using Banks.UI.Queries;
using Banks.UI.Views;
using Terminal.Gui;

namespace Banks.UI.ViewModels
{
    public class AccountsViewModel : ViewModel
    {
        private AccountsView _view;
        private Bank _bank;
        private CentralBank _centralBank;

        public AccountsViewModel(Bank bank, CentralBank centralBank)
        {
            _view = new AccountsView(this);
            _bank = bank;
            _centralBank = centralBank;
            GetAccountsQuery = new BaseQuery<List<BankAccount>>(GetAccounts);
            WithdrawCommand = new BaseParametrizedCommand<SingleAccountBalanceChangeCommandArguments>(Withdraw);
            DepositCommand = new BaseParametrizedCommand<SingleAccountBalanceChangeCommandArguments>(Deposit);
            TransferCommand = new BaseParametrizedCommand<TransferCommandArguments>(Transfer);
        }

        public BaseQuery<List<BankAccount>> GetAccountsQuery { get; }
        public BaseParametrizedCommand<SingleAccountBalanceChangeCommandArguments> WithdrawCommand { get; }
        public BaseParametrizedCommand<SingleAccountBalanceChangeCommandArguments> DepositCommand { get; }
        public BaseParametrizedCommand<TransferCommandArguments> TransferCommand { get; }

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

        private CommandResult Withdraw(SingleAccountBalanceChangeCommandArguments arg)
        {
            var transaction = _centralBank.WithdrawMoney(_bank.Id, arg.AccountId, arg.Amount);
            if (transaction.Status == TransactionStatus.Failed)
                return CommandResult.Fail(transaction.Message);
            return CommandResult.Success();
        }

        private CommandResult Deposit(SingleAccountBalanceChangeCommandArguments arg)
        {
            var transaction = _centralBank.DepositMoney(_bank.Id, arg.AccountId, arg.Amount);
            if (transaction.Status == TransactionStatus.Failed)
                return CommandResult.Fail(transaction.Message);
            return CommandResult.Success();
        }

        private CommandResult Transfer(TransferCommandArguments arg)
        {
            var transaction = _centralBank.TransferMoney(_bank.Id, arg.AccountFromId, arg.AccountToId, arg.Amount);
            if (transaction.Status == TransactionStatus.Failed)
                return CommandResult.Fail(transaction.Message);
            return CommandResult.Success();
        }
    }
}