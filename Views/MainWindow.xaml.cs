using Mail.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;

namespace Mail.Views
{
    public sealed partial class MainWindow : Window
    {
        private readonly NavigationService _navigationService;

        public MainWindow(NavigationService navigationService)
        {
            this.InitializeComponent();

            _navigationService = navigationService;

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
        }

        //NAVIGATION
        private void NavigationView_Loaded(object sender, RoutedEventArgs e) => _navigationService.Initialize(ContentFrame);

        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                _navigationService.Navigate(typeof(SettingsPage), args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null)
            {
                Type? navPageType = Type.GetType(args.InvokedItemContainer.Tag.ToString()!);

                if (navPageType != null)
                    _navigationService.Navigate(navPageType, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) => _navigationService.GoBack();

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            NavigationView.IsBackEnabled = ContentFrame.CanGoBack;

            if (ContentFrame.SourcePageType == typeof(SettingsPage))
            {
                // SettingsItem is not part of navigationView.MenuItems, and doesn't have a Tag.
                NavigationView.SelectedItem = (NavigationViewItem)NavigationView.SettingsItem;
            }
            else if (ContentFrame.SourcePageType is not null && ContentFrame.SourcePageType.FullName is not null)
            {
                // Select the nav view item that corresponds to the page being navigated to.
                NavigationView.SelectedItem = NavigationView.MenuItems
                            .OfType<NavigationViewItem>()
                            .First(i => i.Tag.Equals(ContentFrame.SourcePageType.FullName.ToString()));
            }
        }
    }
}