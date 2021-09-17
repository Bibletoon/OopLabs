using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Shops.Models;
using Shops.Tools;
using Shops.UI.Commands.CommandArguments;
using Shops.UI.ViewModels;
using Terminal.Gui;

namespace Shops.UI.Views
{
    public class BuyProductsView : View<BuyProductsViewModel>
    {
        private readonly List<ProductOrder> _orders = new List<ProductOrder>();
        private List<Shop> _shops = new List<Shop>();
        private List<User> _users = new List<User>();
        private List<Product> _products = new List<Product>();
        private TableView? _ordersTable;
        private Shop? _selectedShop;
        private User? _selectedUser;
        private bool _dialogResult;

        public BuyProductsView(BuyProductsViewModel viewModel)
            : base(viewModel)
        {
        }

        public override void Dispose()
        {
            _ordersTable?.Dispose();
        }

        protected override void InitComponents(Toplevel top)
        {
            _shops = ViewModel.GetAllShopsQuery.Execute();
            _users = ViewModel.GetAllUsersQuery.Execute();
            _products = ViewModel.GetAllProductsQuery.Execute();

            var frame = new FrameView("Buy products")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(1),
                CanFocus = false,
            };

            var userLabel = new Label("User: ")
            {
                X = 0,
                Y = 0,
                TextAlignment = TextAlignment.Right,
            };

            var userComboBox = new ComboBox()
            {
                X = Pos.Right(userLabel) + 1,
                Y = Pos.Top(userLabel),
                Height = 10,
                Width = Dim.Fill(),
            };

            userComboBox.SetSource(_users.Select(s => s.Name).ToList());

            userComboBox.SelectedItemChanged += args => _selectedUser = _users[args.Item];

            var shopLabel = new Label("Shop: ")
            {
                X = 0,
                Y = Pos.Bottom(userLabel),
                TextAlignment = TextAlignment.Right,
            };

            var shopComboBox = new ComboBox()
            {
                X = Pos.Right(shopLabel) + 1,
                Y = Pos.Top(shopLabel),
                Height = 10,
                Width = Dim.Fill(),
            };

            shopComboBox.SetSource(_shops.Select(s => s.Name).ToList());

            shopComboBox.SelectedItemChanged += args => _selectedShop = _shops[args.Item];

            var addOrderButton = new Button("Add Order")
            {
                X = 0,
                Y = Pos.Bottom(shopLabel),
            };

            addOrderButton.Clicked += ShowAddOrderDialog;

            var buyButton = new Button("Buy")
            {
                X = 0,
                Y = Pos.Bottom(addOrderButton),
            };

            buyButton.Clicked += BuyProducts;

            _ordersTable = new TableView()
            {
                X = 0,
                Y = Pos.Bottom(buyButton),
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
            };

            BuildOrdersTable();
            _ordersTable.CellActivated += ExecuteOrderEdit;
            _ordersTable.KeyUp += DeleteOrder;
            AddStatusbarText("~Enter~ on row - Edit order");
            AddStatusbarText("~Delete~ on row - Delete order");

            frame.Add(userLabel, userComboBox, shopLabel, shopComboBox, addOrderButton, buyButton, _ordersTable);
            top.Add(frame);
        }

        private void ShowAddOrderDialog()
        {
            var dialog = new Dialog("Add order", CreateDialogButtons("Add"))
            {
                Width = Dim.Fill(80),
                Height = 7,
            };

            var productLabel = new Label("Product: ")
            {
                X = 0,
                Y = 0,
                Width = 9,
                TextAlignment = TextAlignment.Right,
            };

            var productsComboBox = new ComboBox()
            {
                X = Pos.Right(productLabel) + 1,
                Y = Pos.Top(productLabel),
                Height = 5,
                Width = Dim.Fill(),
            };
            productsComboBox.SetSource(_products.Select(p => p.Name).ToList());

            var productCountLabel = new Label("Product count:")
            {
                X = 0,
                Y = Pos.Bottom(productLabel),
                Width = 14,
                Height = 1,
                TextAlignment = TextAlignment.Right,
            };

            var productCountEdit = new TextField("1")
            {
                X = Pos.Right(productCountLabel) + 1,
                Y = Pos.Top(productCountLabel),
                Width = Dim.Fill(),
                Height = 1,
            };

            dialog.Add(
                       productLabel,
                       productsComboBox,
                       productCountLabel,
                       productCountEdit);

            productsComboBox.SetFocus();

            Application.Run(dialog);

            if (!_dialogResult)
                return;

            if (productsComboBox.SelectedItem < 0)
            {
                ShowError("Product is not selected");
                return;
            }

            bool validateCount = ValidateOrderParameter(productCountEdit.Text.ToString(), out int count);

            if (!validateCount)
            {
                ShowError("Invalid count");
                return;
            }

            var selectedProduct = _products[productsComboBox.SelectedItem];

            if (_orders.Any(l => l.Product == selectedProduct))
            {
                ShowError("Product already added");
                return;
            }

            ProductOrder newOrder = selectedProduct.ToOrder(count);
            _orders.Add(newOrder);
            BuildOrdersTable();
        }

        private void ExecuteOrderEdit(TableView.CellActivatedEventArgs args)
        {
            if (args.Row < 0)
                return;

            var selectedOrder = _orders[args.Row];
            ShowEditOrderDialog(selectedOrder);
        }

        private void ShowEditOrderDialog(ProductOrder orderToEdit)
        {
            _dialogResult = false;
            var dialog = new Dialog("Edit Order", CreateDialogButtons("Edit"))
            {
                Width = Dim.Fill(80),
                Height = 7,
            };

            var productLabel = new Label("Product: ")
            {
                X = 0,
                Y = 0,
                Width = 9,
                TextAlignment = TextAlignment.Right,
            };

            var productTextField = new TextField(orderToEdit.Product.Name)
            {
                X = Pos.Right(productLabel) + 1,
                Y = Pos.Top(productLabel),
                Height = 5,
                Width = Dim.Fill(),
                CanFocus = false,
            };

            var productCountLabel = new Label("Product count:")
            {
                X = 0,
                Y = Pos.Bottom(productLabel),
                Width = 14,
                Height = 1,
                TextAlignment = TextAlignment.Right,
            };

            var productCountEdit = new TextField(orderToEdit.Count.ToString())
            {
                X = Pos.Right(productCountLabel) + 1,
                Y = Pos.Top(productCountLabel),
                Width = Dim.Fill(),
                Height = 1,
            };

            dialog.Add(
                       productLabel,
                       productTextField,
                       productCountLabel,
                       productCountEdit);

            Application.Run(dialog);

            if (!_dialogResult)
                return;

            bool validateCount = ValidateOrderParameter(productCountEdit.Text.ToString(), out int count);

            if (!validateCount)
            {
                ShowError("Invalid order count");
                return;
            }

            int index = _orders.IndexOf(orderToEdit);

            _orders.Remove(orderToEdit);
            _orders.Insert(index, orderToEdit.Product.ToOrder(count));

            BuildOrdersTable();
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

        private bool ValidateOrderParameter(string? fieldData, out int result)
        {
            bool parseParameter = int.TryParse(fieldData, out result);

            if (!parseParameter || result <= 0)
            {
                return false;
            }

            return true;
        }

        private void BuyProducts()
        {
            if (_orders.Count == 0)
            {
                ShowError("You must add Orders");
                return;
            }

            if (_selectedShop is null)
            {
                ShowError("Shop is not selected");
                return;
            }

            if (_selectedUser is null)
            {
                ShowError("User is not selected");
                return;
            }

            var result = ViewModel.BuyProductsCommand.Execute(
                new BuyProductsCommandArguments(_selectedUser, _selectedShop, _orders));

            if (!result.IsSuccess)
            {
                ShowError(result.Message);
                return;
            }

            ViewModel.QuitCommand.Execute();
        }

        private void BuildOrdersTable()
        {
            _ordersTable = _ordersTable.ThrowIfNull(new ApplicationException("Application is not initialised"));

            var table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Count", typeof(int));

            foreach (var order in _orders)
            {
                List<object> row = new List<object>()
                {
                    order.Product.Name,
                    order.Count,
                };

                table.Rows.Add(row.ToArray());
            }

            var nameStyle = new TableView.ColumnStyle()
            {
                MinWidth = 15,
                Alignment = TextAlignment.Centered,
            };

            var countStyle = new TableView.ColumnStyle()
            {
                MinWidth = 5,
                Alignment = TextAlignment.Centered,
            };

            _ordersTable.RemoveAll();
            _ordersTable.Table = table;

            _ordersTable.Style.ColumnStyles.Add(_ordersTable.Table.Columns["Name"], nameStyle);
            _ordersTable.Style.ColumnStyles.Add(_ordersTable.Table.Columns["Count"], countStyle);
        }

        private void DeleteOrder(View.KeyEventEventArgs args)
        {
            if (args.KeyEvent.Key != Key.DeleteChar || _ordersTable is null)
                return;

            if (_ordersTable.SelectedRow < 0)
                return;

            _orders.RemoveAt(_ordersTable.SelectedRow);
            BuildOrdersTable();
        }
    }
}