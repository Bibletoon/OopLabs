using System;
using Banks.Tools;
using Banks.UI.Commands;
using Banks.UI.Views;
using Terminal.Gui;

namespace Banks.UI.ViewModels
{
    public class TimeTravelViewModel : ViewModel
    {
        private readonly TimeTravelView _view;
        private readonly IDateTimeProvider _dateTimeProvider;

        public TimeTravelViewModel(IDateTimeProvider dateTimeProvider)
        {
            _view = new TimeTravelView(this);
            _dateTimeProvider = dateTimeProvider;
            TimeTravel = new BaseParametrizedCommand<string>(DoTimeTravel);
        }

        public BaseParametrizedCommand<string> TimeTravel { get; }

        public override void Dispose()
        {
            _view.Dispose();
        }

        protected override void Init(Toplevel top)
        {
            _view.Init(top);
        }

        private CommandResult DoTimeTravel(string time)
        {
            var res = TimeSpan.TryParse(time, out var span);

            if (!res)
                return CommandResult.Fail("Wrong time format");

            ((DateTimeProvider)_dateTimeProvider).Offset = ((DateTimeProvider)_dateTimeProvider).Offset.Add(span);
            return CommandResult.Success();
        }
    }
}