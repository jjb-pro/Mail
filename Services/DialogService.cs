using Mail.Views;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

namespace Mail.Services;

public class DialogService
{
    private SendPage? _sendPage;
    private ContactsPage? _contactsPage;
    private AccountsPage? _accountsPage;

    public void Initialize(SendPage sendPage) => _sendPage = sendPage;
    public void Initialize(ContactsPage contactsPage) => _contactsPage = contactsPage;
    public void Initialize(AccountsPage accountsPage) => _accountsPage = accountsPage;

    public async Task<ContentDialogResult> ShowEmailSendDialog(string title, string message)
    {
        if (null != _sendPage)
            return await _sendPage.ShowEmailSendDialog(title, message);
        else
            return ContentDialogResult.None;
    }

    public async Task<ContentDialogResult> ShowContactSaveFailureDialog(string errorMessage)
    {
        if (null != _contactsPage)
            return await _contactsPage.ShowContactSaveFailureDialog(errorMessage);
        else
            return ContentDialogResult.None;
    }

    public async Task<ContentDialogResult> ShowAccountSaveFailureDialog(string errorMessage)
    {
        if (null != _accountsPage)
            return await _accountsPage.ShowAccountSaveFailureDialog(errorMessage);
        else
            return ContentDialogResult.None;
    }
}
