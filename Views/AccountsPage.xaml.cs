using Mail.Services;
using Mail.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace Mail.Views;

public sealed partial class AccountsPage : Page
{
    public AccountsViewModel ViewModel { get; } = (Application.Current as App)!.Container!.GetService<AccountsViewModel>()!;

    public AccountsPage()
    {
        this.InitializeComponent();

        (Application.Current as App)!.Container!.GetService<DialogService>()!.Initialize(this);
    }

    // dialogs
    public async Task<ContentDialogResult> ShowAccountSaveFailureDialog(string errorMessage)
    {
        ContentDialog dialog = new()
        {
            XamlRoot = XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = "Error",
            PrimaryButtonText = "OK",
            DefaultButton = ContentDialogButton.Primary,
            Content = $"Failed to save the account data: {errorMessage}"
        };

        return await dialog.ShowAsync();
    }
}