using System.Collections.Generic;
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

        public AccountsViewModel(Bank bank)
        {
            _view = new AccountsView(this);
            _bank = bank;
            GetAccountsQuery = new BaseQuery<List<BankAccount>>(bank.GetAccounts);
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

        private CommandResult Withdraw(SingleAccountBalanceChangeCommandArguments arg)
        {
            var transaction = _bank.WithdrawMoney(arg.AccountId, arg.Amount);
            if (transaction.Status == TransactionStatus.Failed)
                return CommandResult.Fail(transaction.Message);
            return CommandResult.Success();
        }

        private CommandResult Deposit(SingleAccountBalanceChangeCommandArguments arg)
        {
            var transaction = _bank.DepositMoney(arg.AccountId, arg.Amount);
            if (transaction.Status == TransactionStatus.Failed)
                return CommandResult.Fail(transaction.Message);
            return CommandResult.Success();
        }

        private CommandResult Transfer(TransferCommandArguments arg)
        {
            var transaction = _bank.TransferMoney(arg.AccountFromId, arg.AccountToId, arg.Amount);
            if (transaction.Status == TransactionStatus.Failed)
                return CommandResult.Fail(transaction.Message);
            return CommandResult.Success();
        }
    }
}