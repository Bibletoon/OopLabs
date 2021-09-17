using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Shops.Models;
using Shops.Tools;
using Shops.UI.Commands;
using Shops.UI.Commands.CommandArguments;
using Shops.UI.ViewModels;
using Terminal.Gui;

namespace Shops.UI.Views
{
    public class ShopsListView : View<ShopsListViewModel>
    {
        private TableView _lotsTable = new TableView();
        private ListView _shopsListView = new ListView();
        private List<Shop> _shops = new List<Shop>();
        private List<Lot> _currentLots = new List<Lot>();

        public ShopsListView(ShopsListViewModel viewModel)
            : base(viewModel)
        {
        }

        public override void Dispose()
        {
            _lotsTable?.Dispose();
            _shopsListView?.Dispose();
        }

        protected override void InitComponents(Toplevel top)
        {
            _shops = ViewModel.GetAllShopsQuery.Execute();

            var leftPane = new FrameView("Shops")
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

            _shopsListView = new ListView(_shops.Select(s => s.Name).ToList())
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(0),
                Height = Dim.Fill(0),
                AllowsMarking = false,
                CanFocus = true,
            };

            _shopsListView.SelectedItemChanged += ShopsListItemChanged;

            leftPane.Add(_shopsListView);

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

            _lotsTable = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
            };

            _lotsTable.CellActivated += EditProductPrice;
            AddStatusbarText("~Enter~ on row - Edit product price");

            rightPane.Add(_lotsTable);

            _shopsListView.SelectedItem = 0;
            _shopsListView.OnSelectedChanged();

            top.Add(leftPane);
            top.Add(rightPane);
        }

        private void ShopsListItemChanged(ListViewItemEventArgs eventArgs)
        {
            if (_lotsTable is null || _shops is null)
                throw new ApplicationException("App is not initialized");

            var selectedShop = _shops[eventArgs.Item];

            _lotsTable.RemoveAll();

            if (selectedShop is null)
                throw new ApplicationException("Shops load error");

            _currentLots = selectedShop.GetAllProductsInfo().ToList();
            BuildLotsTable();
        }

        private void EditProductPrice(TableView.CellActivatedEventArgs e)
        {
            if (e.Table is null || e.Row < 0)
                return;

            var selectedLot = _currentLots[e.Row];
            bool okPressed = false;

            var okButton = new Button("Ok", is_default: true);

            okButton.Clicked += () =>
                                {
                                    okPressed = true;
                                    Application.RequestStop();
                                };

            var cancelButton = new Button("Cancel");
            cancelButton.Clicked += () => { Application.RequestStop(); };

            var dialog = new Dialog("Enter new value", okButton, cancelButton)
            {
                Width = 50,
                Height = 4,
            };

            var priceLabel = new Label()
            {
                X = 0,
                Y = 0,
                Text = "Price: ",
            };

            var priceField = new TextField()
            {
                Text = selectedLot.Price.ToString(),
                X = Pos.Right(priceLabel) + 1,
                Y = Pos.Top(priceLabel),
                Width = Dim.Fill(),
            };

            dialog.Add(priceLabel, priceField);
            priceField.SetFocus();

            Application.Run(dialog);

            if (!okPressed)
            {
                return;
            }

            bool isInt = int.TryParse(priceField.Text?.ToString(), out int newPrice);

            if (!isInt || newPrice <= 0)
            {
                ShowError("New price is not valid");
                return;
            }

            Shop shop = _shops[_shopsListView.SelectedItem];
            Product product = _currentLots[e.Row].Product;

            var arguments = new ChangeProductPriceArguments(shop, product, newPrice);

            var result = ViewModel.ChangeProductPriceCommand.Execute(arguments);

            if (!result.IsSuccess)
            {
                ShowError(result.Message);
                return;
            }

            e.Table.Rows[e.Row][2] = newPrice.ToString();
            _lotsTable.Update();
        }

        private void BuildLotsTable()
        {
            _lotsTable = _lotsTable.ThrowIfNull(new ApplicationException("Application is not initialised"));

            var table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Count", typeof(int));
            table.Columns.Add("Price", typeof(int));

            foreach (var lot in _currentLots)
            {
                List<object> row = new List<object>()
                {
                    lot.Product.Name,
                    lot.Count,
                    lot.Price,
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

            var priceStyle = new TableView.ColumnStyle()
            {
                MinWidth = 5,
                Alignment = TextAlignment.Centered,
            };

            _lotsTable.RemoveAll();
            _lotsTable.Table = table;

            _lotsTable.Style.ColumnStyles.Add(_lotsTable.Table.Columns["Name"], nameStyle);
            _lotsTable.Style.ColumnStyles.Add(_lotsTable.Table.Columns["Count"], countStyle);
            _lotsTable.Style.ColumnStyles.Add(_lotsTable.Table.Columns["Price"], priceStyle);
        }
    }
}