using Mail.Views;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;

namespace Mail.Services;

public class NavigationService
{
    private Frame? _appFrame;

    public void Initialize(Frame appFrame)
    {
        _appFrame = appFrame;

        Navigate(typeof(SendPage), new EntranceNavigationTransitionInfo());
    }

    public void Navigate(Type navPageType, NavigationTransitionInfo transitionInfo, object? parameter = null)
    {
        // Get the page type before navigation so you can prevent duplicate
        // entries in the backstack.
        Type preNavPageType = _appFrame!.CurrentSourcePageType;

        // Only navigate if the selected page isn't currently loaded.
        if (navPageType is not null && !Type.Equals(preNavPageType, navPageType))
        {
            _appFrame.Navigate(navPageType, parameter, transitionInfo);
        }
    }

    public void GoBack() => TryGoBack();

    private bool TryGoBack()
    {
        if (null == _appFrame || !_appFrame.CanGoBack)
            return false;

        // Don't go back if the nav pane is overlayed.
        //if (navigationView.IsPaneOpen &&
        //    (navigationView.DisplayMode == NavigationViewDisplayMode.Compact ||
        //     navigationView.DisplayMode == NavigationViewDisplayMode.Minimal))
        //    return false;

        _appFrame.GoBack();
        return true;
    }
}