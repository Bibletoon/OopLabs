using Banks.UI.Commands.CommandArguments;
using Banks.UI.ViewModels;
using Terminal.Gui;

namespace Banks.UI.Views
{
    public class CreateClientView : View<CreateClientViewModel>
    {
        private TextField _name;
        private TextField _surname;
        private TextField _address;
        private TextField _passportNumber;

        public CreateClientView(CreateClientViewModel viewModel)
            : base(viewModel)
        {
        }

        public override void Dispose()
        {
            _name?.Dispose();
            _surname?.Dispose();
            _address?.Dispose();
            _passportNumber?.Dispose();
        }

        protected override void InitComponents(Toplevel top)
        {
            var frame = new FrameView("Create client")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(1),
            };

            var nameLabel = new Label("Name: ")
            {
                X = 0,
                Y = 0,
            };

            _name = new TextField()
            {
                X = Pos.Right(nameLabel),
                Y = Pos.Top(nameLabel),
                Width = Dim.Fill(),
            };

            var surname = new Label("Surname: ")
            {
                X = 0,
                Y = Pos.Bottom(nameLabel),
            };

            _surname = new TextField()
            {
                X = Pos.Right(surname),
                Y = Pos.Top(surname),
                Width = Dim.Fill(),
            };

            var address = new Label("Address: ")
            {
                X = 0,
                Y = Pos.Bottom(surname),
            };

            _address = new TextField()
            {
                X = Pos.Right(address),
                Y = Pos.Top(address),
                Width = Dim.Fill(),
            };

            var passportNumberLabel = new Label("Passport number: ")
            {
                X = 0,
                Y = Pos.Bottom(address),
            };

            _passportNumber = new TextField()
            {
                X = Pos.Right(passportNumberLabel),
                Y = Pos.Top(passportNumberLabel),
                Width = Dim.Fill(),
            };

            var button = new Button("Create")
            {
                X = 0,
                Y = Pos.Bottom(passportNumberLabel),
            };

            button.Clicked += CreateClient;

            frame.Add(nameLabel, _name, surname, _surname, address, _address, passportNumberLabel, _passportNumber, button);
            top.Add(frame);
        }

        private void CreateClient()
        {
            var name = _name.Text.ToString();
            var surname = _surname.Text.ToString();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname))
            {
                ShowError("Name and surname is required");
                return;
            }

            var address = _address.Text.ToString();
            var res = uint.TryParse(_passportNumber.Text.ToString(), out var passportNumber);
            if (!string.IsNullOrEmpty(_passportNumber.Text.ToString()) && !res)
            {
                ShowError("Passport number is not valid");
                return;
            }

            var result = ViewModel.CreateUserCommand.Execute(
                new CreateUserCommandArguments(name, surname, address, res ? passportNumber : null));

            if (!result.IsSuccess)
            {
                ShowError(result.Message);
                return;
            }

            ViewModel.QuitCommand.Execute();
        }
    }
}