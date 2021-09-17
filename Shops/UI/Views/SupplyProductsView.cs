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
    public class SupplyProductsView : View<SupplyProductsViewModel>
    {
        private readonly List<Lot> _lots = new List<Lot>();
        private List<Shop> _shops = new List<Shop>();
        private List<Product> _products = new List<Product>();
        private TableView _lotsTable;
        private Shop _selectedShop;
        private bool _dialogResult;

        public SupplyProductsView(SupplyProductsViewModel viewModel)
            : base(viewModel)
        {
        }

        public override void Dispose()
        {
            _lotsTable?.Dispose();
        }

        protected override void InitComponents(Toplevel top)
        {
            _shops = ViewModel.GetAllShopsQuery.Execute();
            _products = ViewModel.GetAllProductsQuery.Execute();

            var frame = new FrameView("Supply products")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(1),
                CanFocus = false,
            };

            var shopLabel = new Label("Shop: ")
            {
                X = 0,
                Y = 0,
                Width = 6,
                TextAlignment = TextAlignment.Right,
            };

            var comboBox = new ComboBox()
            {
                X = Pos.Right(shopLabel) + 1,
                Y = Pos.Top(shopLabel),
                Height = 10,
                Width = Dim.Fill(),
            };

            comboBox.SetSource(_shops.Select(s => s.Name).ToList());

            comboBox.SelectedItemChanged += args => _selectedShop = _shops[args.Item];

            var addLotButton = new Button("Add lot")
            {
                X = 0,
                Y = Pos.Bottom(shopLabel),
            };

            addLotButton.Clicked += ShowAddLotDialog;

            var supplyButton = new Button("Supply")
            {
                X = 0,
                Y = Pos.Bottom(addLotButton),
            };

            supplyButton.Clicked += SupplyProducts;

            _lotsTable = new TableView()
            {
                X = 0,
                Y = Pos.Bottom(supplyButton),
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
            };

            BuildLotsTable();
            _lotsTable.CellActivated += ExecuteLotEdit;
            _lotsTable.KeyPress += DeleteLot;
            AddStatusbarText("~Enter~ on row - Edit lot");
            AddStatusbarText("~Delete~ on row - Delete lot");

            top.Add(frame);
            frame.Add(shopLabel, comboBox, addLotButton, supplyButton, _lotsTable);
        }

        private void ShowAddLotDialog()
        {
            var dialog = new Dialog("Add lot", CreateDialogButtons("Add"))
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

            var productPriceLabel = new Label("Product price:")
            {
                X = 0,
                Y = Pos.Bottom(productCountLabel),
                Width = 14,
                Height = 1,
                TextAlignment = TextAlignment.Right,
            };

            var productPriceEdit = new TextField("1")
            {
                X = Pos.Right(productPriceLabel) + 1,
                Y = Pos.Top(productPriceLabel),
                Width = Dim.Fill(),
                Height = 1,
            };

            dialog.Add(
                       productLabel,
                       productsComboBox,
                       productCountLabel,
                       productCountEdit,
                       productPriceLabel,
                       productPriceEdit);

            productsComboBox.SetFocus();

            Application.Run(dialog);

            if (!_dialogResult)
                return;

            if (productsComboBox.SelectedItem < 0)
            {
                ShowError("Product is not selected");
                return;
            }

            bool validateCount = ValidateLotParameter(productCountEdit.Text.ToString(), out int count);
            bool validatePrice = ValidateLotParameter(productPriceEdit.Text.ToString(), out int price);

            if (!validateCount || !validatePrice)
            {
                ShowError("Invalid lot data");
                return;
            }

            var selectedProduct = _products[productsComboBox.SelectedItem];

            if (_lots.Any(l => l.Product == selectedProduct))
            {
                ShowError("Lot already added");
                return;
            }

            Lot newLot = selectedProduct.ToLot(count, price);
            _lots.Add(newLot);
            BuildLotsTable();
        }

        private void ExecuteLotEdit(TableView.CellActivatedEventArgs args)
        {
            if (args.Row < 0)
                return;

            var selectedLot = _lots[args.Row];
            ShowEditLotDialog(selectedLot);
        }

        private void ShowEditLotDialog(Lot lotToEdit)
        {
            _dialogResult = false;
            var dialog = new Dialog("Edit lot", CreateDialogButtons("Edit"))
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

            var productTextField = new TextField(lotToEdit.Product.Name)
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

            var productCountEdit = new TextField(lotToEdit.Count.ToString())
            {
                X = Pos.Right(productCountLabel) + 1,
                Y = Pos.Top(productCountLabel),
                Width = Dim.Fill(),
                Height = 1,
            };

            var productPriceLabel = new Label("Product price:")
            {
                X = 0,
                Y = Pos.Bottom(productCountLabel),
                Width = 14,
                Height = 1,
                TextAlignment = TextAlignment.Right,
            };

            var productPriceEdit = new TextField(lotToEdit.Price.ToString())
            {
                X = Pos.Right(productPriceLabel) + 1,
                Y = Pos.Top(productPriceLabel),
                Width = Dim.Fill(),
                Height = 1,
            };
            dialog.Add(
                       productLabel,
                       productPriceEdit,
                       productTextField,
                       productCountLabel,
                       productCountEdit,
                       productPriceLabel);

            Application.Run(dialog);

            if (!_dialogResult)
                return;

            bool validateCount = ValidateLotParameter(productCountEdit.Text.ToString(), out int count);
            bool validatePrice = ValidateLotParameter(productPriceEdit.Text.ToString(), out int price);

            if (!validateCount || !validatePrice)
            {
                ShowError("Invalid lot data");
                return;
            }

            int index = _lots.IndexOf(lotToEdit);

            _lots.Remove(lotToEdit);
            _lots.Insert(index, lotToEdit.Product.ToLot(count, price));

            BuildLotsTable();
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

        private bool ValidateLotParameter(string fieldData, out int result)
        {
            bool parseParameter = int.TryParse(fieldData, out result);

            if (!parseParameter || result <= 0)
            {
                return false;
            }

            return true;
        }

        private void SupplyProducts()
        {
            if (_lots.Count == 0)
            {
                ShowError("You must add lots");
                return;
            }

            if (_selectedShop is null)
            {
                ShowError("Shop is not selected");
                return;
            }

            var result = ViewModel.SupplyProductsCommand.Execute(new SupplyProductsCommandArguments(_selectedShop, _lots));

            if (!result.IsSuccess)
            {
                ShowError(result.Message);
                return;
            }

            ViewModel.QuitCommand.Execute();
        }

        private void DeleteLot(View.KeyEventEventArgs args)
        {
            if (args.KeyEvent.Key != Key.DeleteChar || _lotsTable is null)
                return;

            if (_lotsTable.SelectedRow < 0)
                return;

            _lots.RemoveAt(_lotsTable.SelectedRow);
            BuildLotsTable();
        }

        private void BuildLotsTable()
        {
            _lotsTable = _lotsTable.ThrowIfNull(new ApplicationException("Application is not initialised"));

            var table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Count", typeof(int));
            table.Columns.Add("Price", typeof(int));

            foreach (var lot in _lots)
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