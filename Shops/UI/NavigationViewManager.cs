using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Shops.UI.ViewModels;
using Terminal.Gui;

namespace Shops.UI
{
    public class NavigationViewManager
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly Stack<Type> _lastViews;
        private ViewModel? _currentView;

        public NavigationViewManager(ServiceProvider serviceProvider)
        {
            _lastViews = new Stack<Type>();
            _serviceProvider = serviceProvider;
        }

        public void OpenPage(Type pageType)
        {
            if (_serviceProvider.GetService(pageType) is not ViewModel page)
                throw new ApplicationException("Can't open page. Page is not registered or provided not a page.");

            if (_currentView is not null)
                _lastViews.Push(_currentView.GetType());

            Application.Top.RemoveAll();

            _currentView?.Dispose();

            _currentView = page;
            page.Init(Application.Top, this);
        }

        public void Quit()
        {
            if (_lastViews.Count == 0)
            {
                _currentView?.Dispose();
                Application.RequestStop();
            }
            else
            {
                _currentView?.Dispose();
                _currentView = null;
                OpenPage(_lastViews.Pop());
            }
        }
    }
}