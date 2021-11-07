using Banks.UI.ViewModels;
using Terminal.Gui;

namespace Banks.UI.Views
{
    public class PayFeesView : View<PayFeesViewModel>
    {
        public PayFeesView(PayFeesViewModel viewModel)
            : base(viewModel)
        {
        }

        public override void Dispose()
        {
        }

        protected override void InitComponents(Toplevel top)
        {
            var frame = new FrameView("Pay Fees")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(1),
            };

            var button = new Button("Pay")
            {
                X = 0,
                Y = 0,
            };

            button.Clicked += PayFees;

            frame.Add(button);
            top.Add(frame);
        }

        private void PayFees()
        {
            var result = ViewModel.PayFeesCommand.Execute();

            if (result.IsSuccess)
            {
                ViewModel.QuitCommand.Execute();
                return;
            }

            ShowError(result.Message);
        }
    }
}