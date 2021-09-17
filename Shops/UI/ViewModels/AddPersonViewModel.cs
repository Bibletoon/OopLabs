using System;
using Shops.Models;
using Shops.Services;
using Shops.UI.Commands;
using Shops.UI.Commands.CommandArguments;
using Shops.UI.Views;
using Terminal.Gui;

namespace Shops.UI.ViewModels
{
    public class AddPersonViewModel : ViewModel
    {
        private readonly AddPersonView _view;
        private UserManager _userManager;

        public AddPersonViewModel(UserManager userManager)
        {
            _userManager = userManager;

            AddPersonCommand = new BaseParametrizedCommand<AddPersonCommandArguments>(AddPerson);

            _view = new AddPersonView(this);
        }

        public IParameterizedCommand<AddPersonCommandArguments> AddPersonCommand { get; }

        public override void Dispose()
        {
            _view.Dispose();
        }

        protected override void Init(Toplevel top)
        {
            _view.Init(top);
        }

        private CommandResult AddPerson(AddPersonCommandArguments args)
        {
            try
            {
                _userManager.RegisterUser(new User(args.Name, args.Money));
                return CommandResult.Success();
            }
            catch (Exception e)
            {
                return CommandResult.Fail(e.Message);
            }
        }
    }
}