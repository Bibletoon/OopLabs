using Shops.UI.ViewModels;
using Terminal.Gui;

namespace Shops.UI.Views
{
    public class AddShopView : View<AddShopViewModel>
    {
        private TextField _shopNameField;

        public AddShopView(AddShopViewModel viewModel)
            : base(viewModel)
        {
        }

        public override void Dispose()
        {
            _shopNameField?.Dispose();
        }

        protected override void InitComponents(Toplevel top)
        {
            var frame = new FrameView("Add shop")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(1),
                CanFocus = false,
            };

            var shopNameLabel = new Label("Shop name: ")
            {
                X = 0,
                Y = 0,
                Height = 1,
            };

            _shopNameField = new TextField()
            {
                X = Pos.Right(shopNameLabel),
                Y = 0,
                Width = Dim.Fill(),
                Height = 1,
            };

            var createShopButton = new Button("Add shop")
            {
                X = 0,
                Y = Pos.Bottom(shopNameLabel),
            };

            createShopButton.Clicked += CreateShop;

            frame.Add(shopNameLabel, _shopNameField, createShopButton);
            top.Add(frame);
        }

        private void CreateShop()
        {
            var name = _shopNameField.Text.ToString();

            var result = ViewModel.AddShopCommand.Execute(name);

            if (!result.IsSuccess)
            {
                ShowError(result.Message);
                return;
            }

            ViewModel.QuitCommand.Execute();
        }
    }
}