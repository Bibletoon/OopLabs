using System;
using System.Linq;
using Shops.Tools;
using Shops.UI.ViewModels;
using Terminal.Gui;

namespace Shops.UI.Views
{
    public class AddProductView : View<AddProductViewModel>
    {
        private TextField? _productNameField;

        public AddProductView(AddProductViewModel viewModel)
            : base(viewModel)
        {
        }

        public override void Dispose()
        {
            _productNameField?.Dispose();
        }

        protected override void InitComponents(Toplevel top)
        {
            var frame = new FrameView("Add product")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(1),
                CanFocus = false,
            };

            var productNameLabel = new Label("Product name: ")
            {
                X = 0,
                Y = 0,
                Height = 1,
            };

            _productNameField = new TextField()
            {
                X = Pos.Right(productNameLabel),
                Y = 0,
                Width = Dim.Fill(),
                Height = 1,
            };

            var createProductButton = new Button("Add shop")
            {
                X = 0,
                Y = Pos.Bottom(productNameLabel),
            };

            createProductButton.Clicked += CreateProduct;

            frame.Add(productNameLabel, _productNameField, createProductButton);
            top.Add(frame);
        }

        private void CreateProduct()
        {
            var name = _productNameField?.Text.ToString();

            var result = ViewModel.AddProductCommand.Execute(name);

            if (!result.IsSuccess)
            {
                ShowError(result.Message);
                return;
            }

            ViewModel.QuitCommand.Execute();
        }
    }
}