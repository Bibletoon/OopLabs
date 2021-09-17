using System;
using Shops.UI.Commands;
using Terminal.Gui;

namespace Shops.UI.ViewModels
{
    public abstract class ViewModel : IDisposable
    {
        private NavigationViewManager? _viewManager;

        protected ViewModel()
        {
            NavigateToPageCommand = new BaseParametrizedCommand<Type>(pageType =>
                                                                      {
                                                                          _viewManager?.OpenPage(pageType);
                                                                          return CommandResult.Success();
                                                                      });
            QuitCommand = new BaseCommand(() =>
                                          {
                                              _viewManager?.Quit();
                                              return CommandResult.Success();
                                          });
        }

        public IParameterizedCommand<Type> NavigateToPageCommand { get; }
        public ICommand QuitCommand { get; }

        public abstract void Dispose();

        public void Init(Toplevel top, NavigationViewManager navigationViewManager)
        {
            _viewManager = navigationViewManager;
            Init(top);
        }

        protected abstract void Init(Toplevel top);
    }
}