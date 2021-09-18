using System;
using System.Linq;
using Shops.Models;
using Shops.Services;
using Shops.Tools;
using Shops.UI.Commands;
using Shops.UI.Commands.CommandArguments;
using Shops.UI.ViewModels;
using Terminal.Gui;

namespace Shops.UI.Views
{
    public class AddPersonView : View<AddPersonViewModel>
    {
        private TextField _personNameField;
        private TextField _personMoney;

        public AddPersonView(AddPersonViewModel viewModel)
            : base(viewModel)
        {
        }

        public override void Dispose()
        {
            _personNameField?.Dispose();
        }

        protected override void InitComponents(Toplevel top)
        {
            var frame = new FrameView("Add product")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(1),
            };

            var personNameLabel = new Label("User name: ")
            {
                X = 0,
                Y = 0,
                Height = 1,
            };

            _personNameField = new TextField()
            {
                X = Pos.Right(personNameLabel),
                Y = 0,
                Width = Dim.Fill(),
                Height = 1,
            };

            var personMoneyLabel = new Label("User money: ")
            {
                X = 0,
                Y = Pos.Bottom(personNameLabel),
            };

            _personMoney = new TextField()
            {
                X = Pos.Right(personMoneyLabel),
                Y = Pos.Top(personMoneyLabel),
                Width = Dim.Fill(),
            };

            var createPersonButton = new Button("Add person")
            {
                X = 0,
                Y = Pos.Bottom(personMoneyLabel),
            };

            createPersonButton.Clicked += CreatePerson;

            frame.Add(
                      personNameLabel,
                      _personNameField,
                      personMoneyLabel,
                      _personMoney,
                      createPersonButton);
            top.Add(frame);
        }

        private void CreatePerson()
        {
            _personNameField = _personNameField.ThrowIfNull(new ApplicationException("Application is not initialised"));
            var name = _personNameField.Text.ToString();

            bool validMoney = int.TryParse(_personMoney.Text.ToString(), out int money);

            if (!validMoney)
            {
                ShowError("Money is invalid");
                return;
            }

            var result = ViewModel.AddPersonCommand.Execute(new AddPersonCommandArguments(name, money));

            if (!result.IsSuccess)
            {
                ShowError(result.Message);
                return;
            }

            ViewModel.QuitCommand.Execute();
        }
    }
}