using System.Collections.Generic;
using System.Linq;
using Banks.Clients;
using Banks.UI.Commands.CommandArguments;
using Banks.UI.ViewModels;
using Terminal.Gui;

namespace Banks.UI.Views
{
    public class AddAccountView : View<AddAccountViewModel>
    {
        private ComboBox _clientComboBox;
        private TextField _amount;
        private List<Client> _clients;

        public AddAccountView(AddAccountViewModel viewModel)
            : base(viewModel)
        {
        }

        public override void Dispose()
        {
            _clientComboBox?.Dispose();
            _amount?.Dispose();
        }

        protected override void InitComponents(Toplevel top)
        {
            var frame = new FrameView("Add account")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(1),
            };

            var clientLabel = new Label("Client:")
            {
                X = 0,
                Y = 0,
            };

            _clientComboBox = new ComboBox()
            {
                X = Pos.Right(clientLabel),
                Y = Pos.Top(clientLabel),
                Width = Dim.Fill(),
                Height = 10,
            };

            var label = new Label("Amount: ")
            {
                X = 0,
                Y = Pos.Bottom(_clientComboBox),
            };

            _amount = new TextField()
            {
                X = Pos.Right(label),
                Y = Pos.Top(label),
                Width = Dim.Fill(),
            };

            var creditButton = new Button("Create credit")
            {
                X = 0,
                Y = Pos.Bottom(label),
            };

            creditButton.Clicked += CreateCreditAccount;

            var depositButton = new Button("Create deposit")
            {
                X = 0,
                Y = Pos.Bottom(creditButton),
            };

            depositButton.Clicked += CreateDeposit;

            var debitButton = new Button("Create debit")
            {
                X = 0,
                Y = Pos.Bottom(depositButton),
            };

            debitButton.Clicked += CreateDebit;

            _clients = ViewModel.GetClientsQuery.Execute();
            _clientComboBox.SetSource(_clients.Select(c => $"{c.Name} {c.Surname}").ToList());
            frame.Add(clientLabel, _clientComboBox, label, _amount, creditButton, depositButton, debitButton);
            top.Add(frame);
        }

        private void CreateDebit()
        {
            if (_clientComboBox.SelectedItem < 0)
            {
                ShowError("Client is not selected");
                return;
            }

            var res = decimal.TryParse(_amount.Text.ToString(), out var amount);

            if (!res)
            {
                ShowError("Not valid ");
                return;
            }

            var result = ViewModel.CreateDebitAccountCommand.Execute(new CreateAccountCommandArguments(_clients[_clientComboBox.SelectedItem], res ? amount : 0));

            if (!result.IsSuccess)
            {
                ShowError(result.Message);
                return;
            }

            ViewModel.QuitCommand.Execute();
        }

        private void CreateDeposit()
        {
            if (_clientComboBox.SelectedItem < 0)
            {
                ShowError("Client is not selected");
                return;
            }

            var res = decimal.TryParse(_amount.Text.ToString(), out var amount);

            if (!res)
            {
                ShowError("Not valid ");
                return;
            }

            var result = ViewModel.CreateDepositAccountCommand.Execute(new CreateAccountCommandArguments(_clients[_clientComboBox.SelectedItem], res ? amount : 0));

            if (!result.IsSuccess)
            {
                ShowError(result.Message);
                return;
            }

            ViewModel.QuitCommand.Execute();
        }

        private void CreateCreditAccount()
        {
            if (_clientComboBox.SelectedItem < 0)
            {
                ShowError("Client is not selected");
                return;
            }

            var res = decimal.TryParse(_amount.Text.ToString(), out var amount);

            if (!res)
            {
                ShowError("Not valid ");
                return;
            }

            var result = ViewModel.CreateCreditAccountCommand.Execute(new CreateAccountCommandArguments(_clients[_clientComboBox.SelectedItem], res ? amount : 0));

            if (!result.IsSuccess)
            {
                ShowError(result.Message);
                return;
            }

            ViewModel.QuitCommand.Execute();
        }
    }
}