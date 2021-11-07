using Banks.UI.ViewModels;
using Terminal.Gui;

namespace Banks.UI.Views
{
    public class TimeTravelView : View<TimeTravelViewModel>
    {
        private TextField _textInput;

        public TimeTravelView(TimeTravelViewModel viewModel)
            : base(viewModel)
        {
        }

        public override void Dispose()
        {
            _textInput?.Dispose();
        }

        protected override void InitComponents(Toplevel top)
        {
            var frame = new FrameView("Time travel")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(1),
                CanFocus = false,
            };

            var label = new Label("Time span to travel: ")
            {
                X = 0,
                Y = 0,
                Height = 1,
            };

            _textInput = new TextField()
            {
                X = Pos.Right(label),
                Y = 0,
                Width = Dim.Fill(),
                Height = 1,
            };

            var button = new Button("Travel")
            {
                X = 0,
                Y = 1,
            };

            button.Clicked += DoTimeTravel;

            frame.Add(label, _textInput, button);
            top.Add(frame);
        }

        private void DoTimeTravel()
        {
            var result = ViewModel.TimeTravel.Execute(_textInput.Text.ToString());

            if (result.IsSuccess)
            {
                ViewModel.QuitCommand.Execute();
                return;
            }

            ShowError(result.Message);
        }
    }
}