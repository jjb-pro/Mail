using Mail.Services;
using Mail.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace Mail.Views;

public sealed partial class SendPage : Page
{
    public SendViewModel ViewModel { get; } = (Application.Current as App)!.Container!.GetService<SendViewModel>()!;

    public SendPage()
    {
        InitializeComponent();

        (Application.Current as App)!.Container!.GetService<DialogService>()!.Initialize(this);
    }

    // dialogs
    public async Task<ContentDialogResult> ShowEmailSendDialog(string title, string message)
    {
        ContentDialog dialog = new()
        {
            XamlRoot = XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = title,
            PrimaryButtonText = "OK",
            DefaultButton = ContentDialogButton.Primary,
            Content = message
        };

        return await dialog.ShowAsync();
    }
}