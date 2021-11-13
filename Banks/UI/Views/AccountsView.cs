using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Banks.Accounts;
using Banks.UI.Commands.CommandArguments;
using Banks.UI.ViewModels;
using Terminal.Gui;

namespace Banks.UI.Views
{
    public class AccountsView : View<AccountsViewModel>
    {
        private TableView _accountsTable;
        private List<BankAccount> _accounts = new List<BankAccount>();
        private bool _dialogResult = false;

        public AccountsView(AccountsViewModel viewModel)
            : base(viewModel)
        {
        }

        public override void Dispose()
        {
            _accountsTable?.Dispose();
        }

        protected override void InitComponents(Toplevel top)
        {
            var frame = new FrameView("Accounts")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(1),
                CanFocus = false,
                Shortcut = Key.CtrlMask | Key.O,
            };

            _accounts = ViewModel.GetAccountsQuery.Execute();
            _accountsTable = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
            };

            BuildAccountsTable();

            _accountsTable.CellActivated += OperateWithAccount;
            AddStatusbarText("~Enter~ on row - Operate on account");
            frame.Add(_accountsTable);
            top.Add(frame);
        }

        private void OperateWithAccount(TableView.CellActivatedEventArgs obj)
        {
            if (obj.Table is null || obj.Row < 0)
                return;

            var selectedAccount = _accounts[obj.Row];

            var cancelButton = new Button("Cancel");
            cancelButton.Clicked += () => { Application.RequestStop(); };

            var selectOperationDialog = new Dialog("Select operation", cancelButton)
            {
                Width = Dim.Fill(60),
                Height = 20,
            };

            var depositButton = new Button("Deposit")
            {
                X = 0,
                Y = 0,
            };

            depositButton.Clicked += () =>
                                     {
                                         Application.RequestStop();
                                         ShowDepositDialog(selectedAccount);
                                     };

            var withdrawButton = new Button("Withdraw")
            {
                X = 0,
                Y = Pos.Bottom(depositButton),
            };
            withdrawButton.Clicked += () =>
                                      {
                                          Application.RequestStop();
                                          ShowWithdrawDialog(selectedAccount);
                                      };
            var transferButton = new Button("Transfer")
            {
                X = 0,
                Y = Pos.Bottom(withdrawButton),
            };
            transferButton.Clicked += () =>
                                      {
                                          Application.RequestStop();
                                          ShowTransferDialog(selectedAccount);
                                      };
            selectOperationDialog.Add(depositButton, withdrawButton, transferButton);
            depositButton.SetFocus();
            Application.Run(selectOperationDialog);
            _accounts = ViewModel.GetAccountsQuery.Execute();
            BuildAccountsTable();
        }

        private void ShowTransferDialog(BankAccount selectedAccount)
        {
            var dialog = new Dialog("Transfer", CreateDialogButtons("Transfer"))
            {
                Width = Dim.Fill(60),
                Height = 20,
            };

            var label = new Label("Amount: ")
            {
                X = 0,
                Y = 0,
            };

            var amountInput = new TextField()
            {
                X = Pos.Right(label) + 1,
                Y = Pos.Top(label),
                Width = Dim.Fill(),
            };

            var comboBoxLabel = new Label("Account to id")
            {
                X = 0,
                Y = Pos.Bottom(amountInput),
            };

            var comboBox = new ComboBox()
            {
                X = Pos.Right(comboBoxLabel) + 1,
                Y = Pos.Top(comboBoxLabel),
                Height = 10,
                Width = Dim.Fill(),
            };

            BankAccount comboBoxSelectedAccount = null;
            comboBox.SetSource(_accounts.Select(a => a.GetId()).ToList());
            comboBox.SelectedItemChanged += args => comboBoxSelectedAccount = _accounts[args.Item];

            dialog.Add(label, amountInput, comboBoxLabel, comboBox);
            amountInput.SetFocus();
            Application.Run(dialog);

            if (!_dialogResult)
                return;

            var res = decimal.TryParse(amountInput.Text.ToString(), out decimal amount);

            if (!res)
            {
                ShowError("Not valid amount");
                return;
            }

            if (comboBoxSelectedAccount is null)
            {
                ShowError("Account is not selected");
                return;
            }

            var result = ViewModel.TransferCommand.Execute(
            new TransferCommandArguments(selectedAccount.GetId(), comboBoxSelectedAccount.GetId(), amount));

            if (!result.IsSuccess)
            {
                ShowError(result.Message);
            }
        }

        private void ShowWithdrawDialog(BankAccount selectedAccount)
        {
            var dialog = new Dialog("Withdraw", CreateDialogButtons("Withdraw"))
            {
                Width = Dim.Fill(60),
                Height = 20,
            };

            var label = new Label("Amount: ");

            var amountInput = new TextField()
            {
                X = Pos.Right(label) + 1,
                Y = Pos.Top(label),
                Width = Dim.Fill(),
            };

            dialog.Add(label, amountInput);
            amountInput.SetFocus();
            Application.Run(dialog);

            if (!_dialogResult)
                return;

            var res = decimal.TryParse(amountInput.Text.ToString(), out decimal amount);

            if (!res)
            {
                ShowError("Not valid amount");
                return;
            }

            var result = ViewModel.WithdrawCommand.Execute(
                new SingleAccountBalanceChangeCommandArguments(selectedAccount.GetId(), amount));

            if (!result.IsSuccess)
            {
                ShowError(result.Message);
            }
        }

        private void ShowDepositDialog(BankAccount selectedAccount)
        {
            var dialog = new Dialog("Deposit", CreateDialogButtons("Deposit"))
            {
                Width = Dim.Fill(60),
                Height = 20,
            };
            var label = new Label("Amount: ");

            var amountInput = new TextField()
            {
                X = Pos.Right(label) + 1,
                Y = Pos.Top(label),
                Width = Dim.Fill(),
            };

            dialog.Add(label, amountInput);
            amountInput.SetFocus();
            Application.Run(dialog);

            if (!_dialogResult)
                return;

            var res = decimal.TryParse(amountInput.Text.ToString(), out decimal amount);

            if (!res)
            {
                ShowError("Not valid amount");
                return;
            }

            var result = ViewModel.DepositCommand.Execute(
                new SingleAccountBalanceChangeCommandArguments(selectedAccount.GetId(), amount));

            if (!result.IsSuccess)
            {
                ShowError(result.Message);
            }
        }

        private Button[] CreateDialogButtons(string submitButtonText)
        {
            _dialogResult = false;
            var addButton = new Button(submitButtonText);
            var cancelButton = new Button("Cancel");

            addButton.Clicked += () =>
                                 {
                                     _dialogResult = true;
                                     Application.RequestStop();
                                 };

            cancelButton.Clicked += () => { Application.RequestStop(); };

            return new Button[] { addButton, cancelButton };
        }

        private void BuildAccountsTable()
        {
            var dataTable = new DataTable();

            dataTable.Columns.Add("Id", typeof(string));
            dataTable.Columns.Add("Balance", typeof(double));
            dataTable.Columns.Add("Client info", typeof(string));
            foreach (var account in _accounts)
            {
                List<object> row = new List<object>()
                {
                    account.GetId().ToString(),
                    account.GetBalance(),
                    $"{account.GetClient().Name} {account.GetClient().Surname} {account.GetClient().Address} {account.GetClient().PassportNumber}",
                };

                dataTable.Rows.Add(row.ToArray());
            }

            _accountsTable.RemoveAll();
            _accountsTable.Table = dataTable;
        }
    }
}