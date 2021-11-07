using Banks.Banks;
using Banks.UI.Commands;
using Banks.UI.Views;
using Terminal.Gui;

namespace Banks.UI.ViewModels
{
    public class PayFeesViewModel : ViewModel
    {
        private readonly PayFeesView _view;
        private readonly CentralBank _centralBank;

        public PayFeesViewModel(CentralBank centralBank)
        {
            _centralBank = centralBank;
            _view = new PayFeesView(this);
            PayFeesCommand = new BaseCommand(CallFeePayment);
        }

        public BaseCommand PayFeesCommand { get; }

        public override void Dispose()
        {
            _view.Dispose();
        }

        protected override void Init(Toplevel top)
        {
            _view.Init(top);
        }

        private CommandResult CallFeePayment()
        {
            _centralBank.NotifyAboutFeePayment();
            return CommandResult.Success();
        }
    }
}