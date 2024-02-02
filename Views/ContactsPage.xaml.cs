using Mail.Services;
using Mail.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace Mail.Views;

public sealed partial class ContactsPage : Page
{
    public ContactsViewModel ViewModel { get; } = (Application.Current as App)!.Container!.GetService<ContactsViewModel>()!;

    public ContactsPage()
    {
        InitializeComponent();

        (Application.Current as App)!.Container!.GetService<DialogService>()!.Initialize(this);
    }

    // dialogs
    public async Task<ContentDialogResult> ShowContactSaveFailureDialog(string errorMessage)
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