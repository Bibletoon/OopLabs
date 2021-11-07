using System.Collections.Generic;
using System.Linq;
using Banks.UI.Components;
using Banks.UI.ViewModels;
using Terminal.Gui;

namespace Banks.UI.Views
{
    public class MenuView : View<MenuViewModel>
    {
        private readonly List<PanelMenuItem> _scopes;

        private readonly List<PageSelectSubMenuItem> _functions;

        private ListView _scopeList;
        private int _scopeListViewItem;
        private ListView _functionsList;
        private int _functionsListViewItem;

        public MenuView(MenuViewModel menuViewModel)
            : base(menuViewModel)
        {
            _scopeList = new ListView();
            _scopeListViewItem = 0;
            _functionsList = new ListView();
            _functionsListViewItem = 0;

            var systemScope = new PanelMenuItem("System scope");
            var userScope = new PanelMenuItem("User scope");

            _scopes = new List<PanelMenuItem>()
            {
                systemScope,
                userScope,
            };

            _functions = new List<PageSelectSubMenuItem>()
            {
                new PageSelectSubMenuItem("Time travel", systemScope, typeof(TimeTravelViewModel)),
                new PageSelectSubMenuItem("Pay fees", systemScope, typeof(PayFeesViewModel)),
                new PageSelectSubMenuItem("View user accounts", userScope, typeof(AccountsViewModel)),
                new PageSelectSubMenuItem("View transactions", userScope, typeof(TransactionsViewModel)),
                new PageSelectSubMenuItem("Add client", userScope, typeof(CreateClientViewModel)),
                new PageSelectSubMenuItem("Add account", userScope, typeof(AddAccountViewModel)),
            };

            _functions = _functions.OrderBy(ps => ps.ParentItem.Name)
                                   .ThenBy(ps => ps.Name)
                                   .ToList();
        }

        public override void Dispose()
        {
            _scopeList?.Dispose();
            _functionsList?.Dispose();
        }

        protected override void InitComponents(Toplevel top)
        {
            var leftPane = new FrameView("Scope")
            {
                X = 0,
                Y = 0,
                Width = 25,
                Height = Dim.Fill(1),
                CanFocus = false,
                Shortcut = Key.CtrlMask | Key.M,
            };

            leftPane.Title = $"{leftPane.Title} ({leftPane.ShortcutTag})";
            leftPane.ShortcutAction = () => leftPane.SetFocus();

            _scopeList = new ListView(_scopes.Select(s => s.Name).ToList())
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(0),
                Height = Dim.Fill(0),
                AllowsMarking = false,
                CanFocus = true,
            };

            _scopeList.SelectedItemChanged += ScopeListItemChanged;
            leftPane.Add(_scopeList);

            var rightPane = new FrameView("Functions")
            {
                X = 25,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(1),
                CanFocus = true,
                Shortcut = Key.CtrlMask | Key.O,
            };

            rightPane.Title = $"{rightPane.Title} ({rightPane.ShortcutTag}";
            rightPane.ShortcutAction = () => rightPane.SetFocus();

            _functionsList = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(0),
                Height = Dim.Fill(0),
                AllowsMarking = false,
                CanFocus = true,
            };

            _functionsList.OpenSelectedItem += LaunchSelectedFunction;

            rightPane.Add(_functionsList);

            _scopeList.SelectedItem = _scopeListViewItem;
            _scopeList.OnSelectedChanged();

            top.Add(leftPane);
            top.Add(rightPane);
        }

        private void ScopeListItemChanged(ListViewItemEventArgs eventArgs)
        {
            if (_scopeListViewItem != _scopeList.SelectedItem)
                _functionsListViewItem = 0;

            _scopeListViewItem = _scopeList.SelectedItem;

            var selectedScope = _scopes[_scopeListViewItem];

            var visibleFunctions
                = _functions.Where(f => f.ParentItem == selectedScope)
                            .Select(f => f.Name).ToList();

            _functionsList.SetSource(visibleFunctions);
            _functionsList.SelectedItem = _functionsListViewItem;
        }

        private void LaunchSelectedFunction(ListViewItemEventArgs eventArgs)
        {
            var selectedScope = _scopes[_scopeListViewItem];

            PageSelectSubMenuItem selectedFunction = _functions
                .Where(ps => ps.ParentItem == selectedScope)
                .OrderBy(ps => ps.Name).ToList()[_functionsList.SelectedItem];
            ViewModel.NavigateToPageCommand.Execute(selectedFunction.Page);
        }
    }
}