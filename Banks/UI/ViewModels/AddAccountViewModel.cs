using System;
using System.Collections.Generic;
using Banks.Banks;
using Banks.Clients;
using Banks.Commands;
using Banks.UI.Commands;
using Banks.UI.Commands.CommandArguments;
using Banks.UI.Queries;
using Banks.UI.Views;
using Terminal.Gui;

namespace Banks.UI.ViewModels
{
    public class AddAccountViewModel : ViewModel
    {
        private AddAccountView _view;
        private Bank _bank;

        public AddAccountViewModel(ClientService clientService, Bank bank)
        {
            _bank = bank;
            _view = new AddAccountView(this);
            GetClientsQuery = new BaseQuery<List<Client>>(clientService.GetClients);
            CreateDebitAccountCommand = new BaseParametrizedCommand<CreateAccountCommandArguments>(CreateDebitAccount);

            CreateCreditAccountCommand =
                new BaseParametrizedCommand<CreateAccountCommandArguments>(CreateCreditAccount);

            CreateDepositAccountCommand =
                new BaseParametrizedCommand<CreateAccountCommandArguments>(CreateDepositAccount);
        }

        public BaseQuery<List<Client>> GetClientsQuery { get; }
        public BaseParametrizedCommand<CreateAccountCommandArguments> CreateDebitAccountCommand { get; }
        public BaseParametrizedCommand<CreateAccountCommandArguments> CreateDepositAccountCommand { get; }
        public BaseParametrizedCommand<CreateAccountCommandArguments> CreateCreditAccountCommand { get; }

        public override void Dispose()
        {
            _view.Dispose();
        }

        protected override void Init(Toplevel top)
        {
            _view.Init(top);
        }

        private CommandResult CreateDebitAccount(CreateAccountCommandArguments arg)
        {
            try
            {
                _bank.CreateDebitAccount(arg.Client, arg.StartBalance);
                return CommandResult.Success();
            }
            catch (Exception e)
            {
                return CommandResult.Fail(e.Message);
            }
        }

        private CommandResult CreateCreditAccount(CreateAccountCommandArguments arg)
        {
            try
            {
                _bank.CreateCreditAccount(arg.Client, arg.StartBalance);
                return CommandResult.Success();
            }
            catch (Exception e)
            {
                return CommandResult.Fail(e.Message);
            }
        }

        private CommandResult CreateDepositAccount(CreateAccountCommandArguments arg)
        {
            try
            {
                _bank.CreateDepositAccount(arg.Client, arg.StartBalance);
                return CommandResult.Success();
            }
            catch (Exception e)
            {
                return CommandResult.Fail(e.Message);
            }
        }
    }
}