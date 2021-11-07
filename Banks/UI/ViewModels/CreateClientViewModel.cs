using System;
using Banks.Clients;
using Banks.UI.Commands;
using Banks.UI.Commands.CommandArguments;
using Banks.UI.Views;
using Terminal.Gui;

namespace Banks.UI.ViewModels
{
    public class CreateClientViewModel : ViewModel
    {
        private CreateClientView _view;
        private ClientService _clientService;

        public CreateClientViewModel(ClientService clientService)
        {
            _view = new CreateClientView(this);
            _clientService = clientService;
            CreateUserCommand = new BaseParametrizedCommand<CreateUserCommandArguments>(CreateClient);
        }

        public BaseParametrizedCommand<CreateUserCommandArguments> CreateUserCommand { get; }

        public override void Dispose()
        {
            _view.Dispose();
        }

        protected override void Init(Toplevel top)
        {
            _view.Init(top);
        }

        private CommandResult CreateClient(CreateUserCommandArguments arg)
        {
            try
            {
                var builder = _clientService.CreateClientBuilder(arg.Name, arg.Surname);

                if (!string.IsNullOrEmpty(arg.Address))
                    builder.SetAddress(arg.Address);

                if (arg.PassportNumber is not null)
                {
                    builder.SetPassportNumber(arg.PassportNumber.Value);
                }

                var client = builder.Build();
                _clientService.RegisterClient(client);
                return CommandResult.Success();
            }
            catch (Exception e)
            {
                return CommandResult.Fail(e.Message);
            }
        }
    }
}