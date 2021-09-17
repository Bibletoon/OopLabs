using System;
using Shops.Models;
using Shops.UI.ViewModels;
using Terminal.Gui;

namespace Shops.UI.Views
{
    public abstract class View<T> : IDisposable
        where T : ViewModel
    {
        private StatusBar? _statusBar;

        protected View(T viewModel)
        {
            ViewModel = viewModel;
        }

        // Из-за бана протектед полей аналайзером, пришлось прибегнуть к такому...
        protected T ViewModel { get; }

        public abstract void Dispose();

        public void Init(Toplevel top)
        {
            _statusBar = new StatusBar()
            {
                Visible = true,
                CanFocus = false,
            };

            _statusBar.Items = new StatusItem[]
            {
                new StatusItem(Key.Q | Key.CtrlMask, "~CTRL~-Q Quit", () =>
                                                                          ViewModel.QuitCommand.Execute()),
            };

            InitComponents(top);

            top.Add(_statusBar);
        }

        protected abstract void InitComponents(Toplevel top);

        protected void ShowError(string message)
        {
            MessageBox.ErrorQuery(
                60,
                5,
                "Error",
                message,
                "Ok");
        }

        protected void AddStatusbarText(string text)
        {
            _statusBar?.AddItemAt(_statusBar.Items.Length, new StatusItem(Key.Null, text, () => { }));
        }
    }
}