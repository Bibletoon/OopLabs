using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Banks.Accounts;
using Banks.Transactions;
using Banks.UI.ViewModels;
using Terminal.Gui;

namespace Banks.UI.Views
{
    public class TransactionsView : View<TransactionsViewModel>
    {
        private List<BankAccount> _accounts;
        private ListView _accountsListView;
        private TableView _transactionsTable;
        private List<Transaction> _currentTransactions;

        public TransactionsView(TransactionsViewModel viewModel)
            : base(viewModel)
        {
        }

        public override void Dispose()
        {
            _accountsListView?.Dispose();
            _transactionsTable?.Dispose();
        }

        protected override void InitComponents(Toplevel top)
        {
            _accounts = ViewModel.GetAccountsQuery.Execute();

            var leftPane = new FrameView("Accounts")
            {
                X = 0,
                Y = 0,
                Width = 25,
                Height = Dim.Fill(1),
                CanFocus = false,
                Shortcut = Key.CtrlMask | Key.M,
            };

            leftPane.Title = $"{leftPane.Title} ({leftPane.ShortcutTag})";
            leftPane.ShortcutAction = () => leftPane.SetFocus();

            _accountsListView = new ListView(_accounts.Select(a => a.GetId()).ToList())
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(0),
                Height = Dim.Fill(0),
                AllowsMarking = false,
                CanFocus = true,
            };

            _accountsListView.SelectedItemChanged += AccountListItemChanged;

            leftPane.Add(_accountsListView);

            var rightPane = new FrameView("Functions")
            {
                X = 25,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(1),
                CanFocus = false,
                Shortcut = Key.CtrlMask | Key.O,
            };

            rightPane.Title = $"{rightPane.Title} ({rightPane.ShortcutTag}";
            rightPane.ShortcutAction = () => rightPane.SetFocus();

            _transactionsTable = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
            };

            _transactionsTable.CellActivated += RevertTransaction;
            AddStatusbarText("~Enter~ on row - Cancel transaction");

            rightPane.Add(_transactionsTable);

            _accountsListView.SelectedItem = 0;
            _accountsListView.OnSelectedChanged();

            top.Add(leftPane);
            top.Add(rightPane);
        }

        private void RevertTransaction(TableView.CellActivatedEventArgs obj)
        {
            if (obj.Row < 0 || _currentTransactions is null)
                return;

            var transaction = _currentTransactions[obj.Row];
            var result = ViewModel.RevertTransactionCommand.Execute(transaction.Id);

            if (!result.IsSuccess)
            {
                ShowError(result.Message);
                return;
            }

            _accountsListView.OnSelectedChanged();
        }

        private void AccountListItemChanged(ListViewItemEventArgs obj)
        {
            if (_transactionsTable is null || _accounts is null)
                throw new ApplicationException("App is not initialized");

            var selectedAccount = _accounts[obj.Item];

            _transactionsTable.RemoveAll();

            if (selectedAccount is null)
                throw new ApplicationException("Shops load error");

            _currentTransactions = ViewModel.GetTransactionsByGuid.Execute(selectedAccount.GetId());
            BuildLotsTable();
        }

        private void BuildLotsTable()
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(string));
            table.Columns.Add("DateTime", typeof(string));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("Message", typeof(string));

            foreach (var transaction in _currentTransactions)
            {
                var row = new List<object>()
                {
                    transaction.Id.ToString(),
                    transaction.TransactionDateTime.ToShortDateString(),
                    transaction.Status.ToString(),
                    transaction.Message,
                };

                table.Rows.Add(row.ToArray());
            }

            _transactionsTable.RemoveAll();
            _transactionsTable.Table = table;
        }
    }
}